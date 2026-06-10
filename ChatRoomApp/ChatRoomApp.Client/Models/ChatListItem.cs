namespace ChatRoomApp.Client.Models;

public class ChatListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LastMessageText { get; set; } = string.Empty;
    public DateTime LastMessageTime { get; set; }
    public string ChatType { get; set; } = string.Empty;
    public int OtherUserId { get; set; }
    public string? OtherUserFullName { get; set; }
    public bool IsOnline { get; set; }
}
