namespace Domain.Entities;

public class GroupMessage : Message
{
    public int GroupChatId { get; set; }
    public virtual GroupChat GroupChat { get; set; } = null!;
}