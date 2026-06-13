using Application.Messages.UseCases.Dto;

namespace ChatRoomApp.Services;

public class MessageBroadcastService
{
    public event Action<MessageDto>? OnPersonalMessageReceived;
    public event Action<MessageDto>? OnGroupMessageReceived;
    public event Action<MessageDto>? OnMessageEdited;
    public event Action<int, int?, int?>? OnMessageDeleted;

    public void NotifyPersonalMessage(MessageDto message)
    {
        OnPersonalMessageReceived?.Invoke(message);
    }

    public void NotifyGroupMessage(MessageDto message)
    {
        OnGroupMessageReceived?.Invoke(message);
    }

    public void NotifyMessageEdited(MessageDto message)
    {
        OnMessageEdited?.Invoke(message);
    }

    public void NotifyMessageDeleted(int messageId, int? groupChatId, int? personalChatId)
    {
        OnMessageDeleted?.Invoke(messageId, groupChatId, personalChatId);
    }
}
