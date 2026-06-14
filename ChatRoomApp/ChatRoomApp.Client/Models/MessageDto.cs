namespace ChatRoomApp.Client.Models;

public class MessageDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentTime { get; set; }
    public bool IsDelivered { get; set; }
    public bool IsSeen { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeliveredTime { get; set; }
    public DateTime? SeenTime { get; set; }
    public DateTime? EditedTime { get; set; }
    public DateTime? DeletedTime { get; set; }
    public int SenderId { get; set; }
    public int? ReceiverId { get; set; }
    public int? PersonalChatId { get; set; }
    public int? GroupChatId { get; set; }
    public int? ReplyToMessageId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string? SenderProfilePictureUrl { get; set; }
    public bool IsPersonalMessage => PersonalChatId.HasValue;
    public bool IsGroupMessage => GroupChatId.HasValue;
}
