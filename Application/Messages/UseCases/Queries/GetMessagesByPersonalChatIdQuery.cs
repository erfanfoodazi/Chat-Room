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
    public class GetMessagesByPersonalChatIdQuery : IRequest<List<MessageDto>>
    {
        public int PersonalId { get; set; }
    }

    public class GetMessagesByPesonalChatIdQueryHandler : IRequestHandler<GetMessagesByPersonalChatIdQuery, List<MessageDto>>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        public GetMessagesByPesonalChatIdQueryHandler(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }
        public async Task<List<MessageDto>> Handle(GetMessagesByPersonalChatIdQuery request, CancellationToken cancellationToken)
        {
            var messages = await _messageRepository.GetMessagesByPersonalChatIdAsync(request.PersonalId);
            if (messages == null || !messages.Any())
                return new List<MessageDto>();

            var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
            var senderNames = new Dictionary<int, string>();
            var senderPictures = new Dictionary<int, string?>();
            foreach (var sid in senderIds)
            {
                var u = await _userRepository.GetUserByIdAsync(sid);
                senderNames[sid] = u?.FullName ?? u?.UserName ?? string.Empty;
                senderPictures[sid] = u?.ProfilePictureUrl;
            }

            return messages.Select(message => new MessageDto()
            {
                Id = message.Id,
                Text = message.Text,
                SenderId = message.SenderId,
                SenderName = senderNames.GetValueOrDefault(message.SenderId, string.Empty),
                SenderProfilePictureUrl = senderPictures.GetValueOrDefault(message.SenderId),
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
