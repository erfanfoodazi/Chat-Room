using Application.Interfaces;
using Application.Messages.UseCases.Dto;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.UseCases.Commands
{
    public class CreateMessageCommand : IRequest<MessageDto>
    {
        public string Text { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public int? PersonalChatId { get; set; } 
        public int? GroupChatId { get; set; }
        public int? ReplyToMessageId { get; set; }
    }

    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, MessageDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        public CreateMessageCommandHandler(IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public async Task<MessageDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var sender = await _userRepository.GetUserByIdAsync(request.SenderId);
            User? receiver = null;
            if (request.ReceiverId.HasValue)
                receiver = await _userRepository.GetUserByIdAsync(request.ReceiverId.Value);

            if (sender == null)
                return null;

            if (!request.PersonalChatId.HasValue && !request.GroupChatId.HasValue)
                throw new Exception("Message must belong to either a personal chat or a group chat");

            if (request.ReceiverId.HasValue && receiver == null)
                return null;

            var message = new Message()
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                PersonalChatId = request.PersonalChatId,
                GroupChatId = request.GroupChatId,
                ReplyToMessageId = request.ReplyToMessageId,
                Text = request.Text,
            };


            var messageData = await _messageRepository.CreateMessageAsync(message);

            var result = new MessageDto()
            {
                Id = messageData.Id,
                SenderId = messageData.SenderId,
                ReceiverId = messageData.ReceiverId,
                PersonalChatId = messageData.PersonalChatId,
                GroupChatId = messageData.GroupChatId,
                ReplyToMessageId = messageData.ReplyToMessageId,
                Text = messageData.Text,
                DeletedTime = messageData.DeletedTime,
                DeliveredTime = messageData.DeliveredTime,
                EditedTime = messageData.EditedTime,
                SeenTime = messageData.SeenTime,
                SentTime = messageData.SentTime,
                IsDeleted = messageData.IsDeleted,
                IsDelivered = messageData.IsDelivered,
                IsEdited = messageData.IsEdited,
                IsSeen = messageData.IsSeen,
            };

            return result;
        }
    }
}
