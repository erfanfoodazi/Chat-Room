namespace ChatRoomApp.Client.Models;

public class GroupMemberDto
{
    public int GroupChatId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public DateTime? LastReadTime { get; set; }
}
