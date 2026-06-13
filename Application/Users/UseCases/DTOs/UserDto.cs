using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.UseCases.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Role {  get; set; }
        public virtual ICollection<PersonalChat> PersonalChats { get; set; } = new List<PersonalChat>();
        public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    }
}
