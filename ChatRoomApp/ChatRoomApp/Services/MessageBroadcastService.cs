using Application.Messages.UseCases.Dto;

namespace ChatRoomApp.Services;

public class MessageBroadcastService
{
    public event Action<MessageDto>? OnPersonalMessageReceived;
    public event Action<MessageDto>? OnGroupMessageReceived;

    public void NotifyPersonalMessage(MessageDto message)
    {
        OnPersonalMessageReceived?.Invoke(message);
    }

    public void NotifyGroupMessage(MessageDto message)
    {
        OnGroupMessageReceived?.Invoke(message);
    }
}
