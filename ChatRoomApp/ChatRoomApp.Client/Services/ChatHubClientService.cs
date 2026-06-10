using ChatRoomApp.Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;

namespace ChatRoomApp.Client.Services;

public class ChatHubClientService : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly NavigationManager _navigation;

    public event Action<MessageDto>? OnPersonalMessageReceived;
    public event Action<MessageDto>? OnGroupMessageReceived;
    public event Action<int>? OnUserConnected;
    public event Action<int>? OnUserDisconnected;
    public event Action<int, int>? OnUserJoinedGroup;
    public event Action<int, int>? OnUserLeftGroup;
    public event Action<int>? OnMessageSeen;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public ChatHubClientService(NavigationManager navigation)
    {
        _navigation = navigation;
    }

    public async Task StartAsync()
    {
        if (_hubConnection != null) return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigation.ToAbsoluteUri("/chathub"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<MessageDto>("ReceivePersonalMessage", msg =>
            OnPersonalMessageReceived?.Invoke(msg));

        _hubConnection.On<MessageDto>("ReceiveGroupMessage", msg =>
            OnGroupMessageReceived?.Invoke(msg));

        _hubConnection.On<int>("UserConnected", userId =>
            OnUserConnected?.Invoke(userId));

        _hubConnection.On<int>("UserDisconnected", userId =>
            OnUserDisconnected?.Invoke(userId));

        _hubConnection.On<int, int>("UserJoinedGroup", (userId, groupId) =>
            OnUserJoinedGroup?.Invoke(userId, groupId));

        _hubConnection.On<int, int>("UserLeftGroup", (userId, groupId) =>
            OnUserLeftGroup?.Invoke(userId, groupId));

        _hubConnection.On<int>("MessageSeen", messageId =>
            OnMessageSeen?.Invoke(messageId));

        await _hubConnection.StartAsync();
    }

    // ─── Personal Chat ───────────────────────────────────────
    public async Task SendPersonalMessage(int receiverId, string text, int? replyToMessageId = null)
    {
        if (_hubConnection is null) return;
        await _hubConnection.InvokeAsync("SendPersonalMessage", receiverId, text, replyToMessageId);
    }

    // ─── Group Chat ───────────────────────────────────────────
    public async Task JoinGroup(int groupId)
    {
        if (_hubConnection is null) return;
        await _hubConnection.InvokeAsync("JoinGroup", groupId);
    }

    public async Task LeaveGroup(int groupId)
    {
        if (_hubConnection is null) return;
        await _hubConnection.InvokeAsync("LeaveGroup", groupId);
    }

    public async Task SendGroupMessage(int groupId, string text, int? replyToMessageId = null)
    {
        if (_hubConnection is null) return;
        await _hubConnection.InvokeAsync("SendGroupMessage", groupId, text, replyToMessageId);
    }

    // ─── Message Status ───────────────────────────────────────
    public async Task MarkMessageDelivered(int messageId)
    {
        if (_hubConnection is null) return;
        await _hubConnection.InvokeAsync("MarkMessageDelivered", messageId);
    }

    public async Task MarkMessageSeen(int messageId, int senderId)
    {
        if (_hubConnection is null) return;
        await _hubConnection.InvokeAsync("MarkMessageSeen", messageId, senderId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
