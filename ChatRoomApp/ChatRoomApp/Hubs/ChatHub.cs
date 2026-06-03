using Application.GroupChats.UseCases.Commands;
using Application.Messages.UseCases.Commands;
using Application.PersonalChats.UseCases.Commands;
using Application.Users.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatRoomApp.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ─── Connection ───────────────────────────────────────────
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId != 0)
        {
            await _mediator.Send(new SetOnlineOrOfflineCommand { UserId = userId });
            await Clients.Others.SendAsync("UserConnected", userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId != 0)
        {
            await _mediator.Send(new SetOnlineOrOfflineCommand { UserId = userId });
            await Clients.Others.SendAsync("UserDisconnected", userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    // ─── Personal Chat ────────────────────────────────────────
    public async Task SendPersonalMessage(int receiverId, string text, int? replyToMessageId = null)
    {
        var senderId = GetUserId();
        var message = await _mediator.Send(new SendPersonalMessageCommand
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Text = text,
            ReplyToMessageId = replyToMessageId,
        });

        // Send to receiver
        await Clients.User(receiverId.ToString()).SendAsync("ReceivePersonalMessage", message);
        // Send back to sender
        await Clients.Caller.SendAsync("ReceivePersonalMessage", message);
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

        await Clients.Group(groupId.ToString()).SendAsync("ReceiveGroupMessage", message);
    }

    // ─── Message Status ───────────────────────────────────────
    public async Task MarkMessageDelivered(int messageId)
    {
        await _mediator.Send(new MakeMessageDeliveredCommand { MessageId = messageId });
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
        return claim != null ? int.Parse(claim.Value) : 0;
    }
}
