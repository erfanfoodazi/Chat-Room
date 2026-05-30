using Application.Interfaces;
using Application.Messages.UseCases.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.UseCases.Commands
{
    public class UpdateMessageCommand : IRequest<MessageDto>
    {
        public int MessageId { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;
        public UpdateMessageCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<MessageDto> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
        {
            var oldMessage = await _messageRepository.GetMessageByIdAsync(request.MessageId);
            if (oldMessage == null)
                return null;

            oldMessage.Text = request.Text;
            oldMessage.EditedTime = DateTime.UtcNow;

            var message = await _messageRepository.EditMessageAsync(oldMessage);

            return new MessageDto()
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
