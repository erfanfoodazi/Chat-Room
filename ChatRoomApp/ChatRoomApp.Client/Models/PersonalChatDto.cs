namespace ChatRoomApp.Client.Models;

public class PersonalChatDto
{
    public int Id { get; set; }
    public int UserOneId { get; set; }
    public int UserTwoId { get; set; }
    public List<MessageDto> Messages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageTime { get; set; }
    public string? LastMessageText { get; set; }
    public bool IsBlocked { get; set; }
    public int? BlockedByUserId { get; set; }
}
