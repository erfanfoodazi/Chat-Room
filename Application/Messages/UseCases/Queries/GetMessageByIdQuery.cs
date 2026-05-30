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
    public class GetMessageByIdQuery : IRequest<MessageDto>
    {
        public int MessageId { get; set; }
    }

    public class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQuery, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;
        public GetMessageByIdQueryHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<MessageDto> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetMessageByIdAsync(request.MessageId);

            if (message == null)
                return null;

            return new MessageDto
            {
                Id = message.Id,
                Text = message.Text,
                EditedTime = message.EditedTime,
                DeletedTime = message.DeletedTime,
                DeliveredTime = message.DeliveredTime,
                SeenTime = message.SeenTime,
                SentTime = message.SentTime,
                GroupChatId = message.GroupChatId,
                PersonalChatId = message.PersonalChatId,
                ReceiverId = message.ReceiverId,
                ReplyToMessageId = message.ReplyToMessageId,
                SenderId = message.SenderId,
                IsDeleted = message.IsDeleted,
                IsDelivered = message.IsDelivered,
                IsEdited = message.IsEdited,
                IsSeen = message.IsSeen,
            };
        }
    }
}
