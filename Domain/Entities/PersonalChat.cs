namespace Domain.Entities;

public class PersonalChat
{
    public int Id { get; set; }

    public int UserOneId { get; set; }
    public virtual User UserOne { get; set; } = null!; 

    public int UserTwoId { get; set; }
    public virtual User UserTwo { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageTime { get; set; }
    public string? LastMessageText { get; set; }

    public bool IsArchived { get; set; }
    public bool IsBlocked { get; set; }
    public int? BlockedByUserId { get; set; }

    public User GetOtherUser(int currentUserId)
    {
        return UserOneId == currentUserId ? UserTwo : UserOne;
    }

    public void UpdateLastMessage(Message message)
    {
        LastMessageTime = message.SentTime;
        LastMessageText = message.Text.Length > 50
            ? message.Text[..50] + "..."
            : message.Text;
    }
}