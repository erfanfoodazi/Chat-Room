using Application.Interfaces;
using Application.Messages.UseCases.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.UseCases.Queries
{
    public class GetMessagesByGroupChatIdQuery : IRequest<List<MessageDto>>
    {
        public int GroupId { get; set; }
    }

    public class GetMessagesByGrpupChatIdQueryHandler : IRequestHandler<GetMessagesByGroupChatIdQuery, List<MessageDto>>
    {
        private readonly IMessageRepository _messageRepository;
        public GetMessagesByGrpupChatIdQueryHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<List<MessageDto>> Handle(GetMessagesByGroupChatIdQuery request, CancellationToken cancellationToken)
        {
            var messages = await _messageRepository.GetMessagesByGroupChatIdAsync(request.GroupId);
            if (messages == null || !messages.Any())
                return new List<MessageDto>();

            return messages.Select(message => new MessageDto()
            {
                Id = message.Id,
                Text = message.Text,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                PersonalChatId = message.PersonalChatId,
                GroupChatId = message.GroupChatId,
                ReplyToMessageId = message.ReplyToMessageId,
                SentTime = message.SentTime,
                DeliveredTime = message.DeliveredTime,
                SeenTime = message.SeenTime,
                EditedTime = message.EditedTime,
                DeletedTime = message.DeletedTime,
                IsDelivered = message.IsDelivered,
                IsSeen = message.IsSeen,
                IsEdited = message.IsEdited,
                IsDeleted = message.IsDeleted,
            }).ToList();
        }
    }
}
