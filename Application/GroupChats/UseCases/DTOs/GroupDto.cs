using Application.Messages.UseCases.Dto;
using Domain.Entities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.GroupChats.UseCases.DTOs
{
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

        public ICollection<GroupMemberDto> Members { get; set; } = new List<GroupMemberDto>();

        public ICollection<MessageDto> Messages { get; set; } = new List<MessageDto>();

    }
}
