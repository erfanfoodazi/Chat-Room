using Application.GroupChats.UseCases.Commands;
using Application.Interfaces;
using Application.Messages.UseCases.Commands;
using Application.Messages.UseCases.Dto;
using Application.PersonalChats.UseCases.Commands;
using Application.Users.UseCases.Commands;
using ChatRoomApp.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatRoomApp.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private readonly MessageBroadcastService _broadcastService;
    private readonly UserStatusBroadcastService _statusBroadcastService;
    private readonly ConnectionTracker _connectionTracker;
    private readonly IPersonalChatRepository _personalChatRepository;

    public ChatHub(IMediator mediator, MessageBroadcastService broadcastService, UserStatusBroadcastService statusBroadcastService, ConnectionTracker connectionTracker, IPersonalChatRepository personalChatRepository)
    {
        _mediator = mediator;
        _broadcastService = broadcastService;
        _statusBroadcastService = statusBroadcastService;
        _connectionTracker = connectionTracker;
        _personalChatRepository = personalChatRepository;
    }

    // ─── Connection ───────────────────────────────────────────
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId != 0)
        {
            var count = _connectionTracker.AddConnection(userId, Context.ConnectionId);
            if (count == 1)
            {
                await _mediator.Send(new SetOnlineOrOfflineCommand { UserId = userId, IsOnline = true });
                await Clients.Others.SendAsync("UserConnected", userId);
                _statusBroadcastService.NotifyStatusChanged(userId, true);
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId != 0)
        {
            var count = _connectionTracker.RemoveConnection(userId, Context.ConnectionId);
            if (count == 0)
            {
                await _mediator.Send(new SetOnlineOrOfflineCommand { UserId = userId, IsOnline = false });
                await Clients.Others.SendAsync("UserDisconnected", userId);
                _statusBroadcastService.NotifyStatusChanged(userId, false);
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    // ─── Personal Chat ────────────────────────────────────────
    public async Task SendPersonalMessage(int receiverId, string text, int? replyToMessageId = null)
    {
        var senderId = GetUserId();
        if (senderId == 0) return;

        var personalChatId = await _personalChatRepository.ExistChat(senderId, receiverId);
        if (personalChatId == 0)
            return;

        var message = await _mediator.Send(new SendPersonalMessageCommand
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            PersonalChatId = personalChatId,
            Text = text,
            ReplyToMessageId = replyToMessageId,
        });

        if (message == null) return;

        // Send to receiver
        await Clients.User(receiverId.ToString()).SendAsync("ReceivePersonalMessage", message);
        // Send back to sender
        await Clients.Caller.SendAsync("ReceivePersonalMessage", message);

        // Notify server-side Blazor components
        _broadcastService.NotifyPersonalMessage(message);
    }

    // ─── Group Chat ───────────────────────────────────────────
    public async Task JoinGroup(int groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        await Clients.Group(groupId.ToString()).SendAsync("UserJoinedGroup", GetUserId(), groupId);
    }

    public async Task LeaveGroup(int groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
        await Clients.Group(groupId.ToString()).SendAsync("UserLeftGroup", GetUserId(), groupId);
    }

    public async Task SendGroupMessage(int groupId, string text, int? replyToMessageId = null)
    {
        var senderId = GetUserId();
        var message = await _mediator.Send(new SendMessageToGroupCommand
        {
            GroupId = groupId,
            SenderId = senderId,
            Text = text,
            ReplyToMessageId = replyToMessageId,
        });

        if (message == null) return;

        await Clients.Group(groupId.ToString()).SendAsync("ReceiveGroupMessage", message);

        // Notify server-side Blazor components
        _broadcastService.NotifyGroupMessage(message);
    }

    // ─── Message Status ───────────────────────────────────────
    public async Task MarkMessageDelivered(int messageId)
    {
        await _mediator.Send(new MakeMessageDeliveredCommand { MessageId = messageId });
        await Clients.Caller.SendAsync("MessageDelivered", messageId);
    }

    public async Task MarkMessageSeen(int messageId, int senderId)
    {
        await _mediator.Send(new MakeMessageSeenCommand { MessageId = messageId });
        await Clients.User(senderId.ToString()).SendAsync("MessageSeen", messageId);
    }

    // ─── Helper ───────────────────────────────────────────────
    private int GetUserId()
    {
        var claim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out var userId) ? userId : 0;
    }
}
