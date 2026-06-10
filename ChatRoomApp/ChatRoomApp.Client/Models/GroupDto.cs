namespace ChatRoomApp.Client.Models;

public class GroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? LastMessageText { get; set; }
    public DateTime? LastMessageTime { get; set; }
    public int OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPublic { get; set; }
    public List<GroupMemberDto> Members { get; set; } = new();
    public List<MessageDto> Messages { get; set; } = new();
}
