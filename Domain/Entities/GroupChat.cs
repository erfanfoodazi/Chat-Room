namespace Domain.Entities;

public class GroupChat
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public int OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublic { get; set; } = true;

    public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}


