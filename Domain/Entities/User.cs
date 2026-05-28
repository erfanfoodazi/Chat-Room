using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsOnline { get; set; }
    public string Bio { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

    public virtual ICollection<PersonalChat> PersonalChats { get; set; } = new List<PersonalChat>();
    public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();

    public void UpdateLastSeen()
    {
        LastSeen = DateTime.UtcNow;
        IsOnline = false;
    }

    public void SetOnline()
    {
        IsOnline = true;
        LastSeen = DateTime.UtcNow;
    }
}