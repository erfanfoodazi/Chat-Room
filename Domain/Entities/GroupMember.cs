namespace Domain.Entities;

public class GroupMember
{
    public int Id { get; set; }
    public int GroupChatId { get; set; }
    public virtual GroupChat GroupChat { get; set; } = null!;

    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public GroupRole Role { get; set; } = GroupRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastReadTime { get; set; }
}
