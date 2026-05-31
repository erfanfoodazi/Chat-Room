using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.GroupChats.UseCases.DTOs
{
    public class GroupMemberDto
    {
        public int GroupChatId { get; set; }

        public int UserId { get; set; }

        public GroupRole Role { get; set; } 
        public DateTime JoinedAt { get; set; } 
        public DateTime? LastReadTime { get; set; }
    }
}
