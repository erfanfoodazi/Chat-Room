using Application.Messages.UseCases.Dto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PersonalChats.UseCases
{
    public class PersonalChatDto
    {
        public int Id { get; set; }

        public int UserOneId { get; set; }
        public int UserTwoId { get; set; }
        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastMessageTime { get; set; }
        public string? LastMessageText { get; set; }
        public bool IsArchived { get; set; }
        public bool IsBlocked { get; set; }
        public int? BlockedByUserId { get; set; }

    }
}
