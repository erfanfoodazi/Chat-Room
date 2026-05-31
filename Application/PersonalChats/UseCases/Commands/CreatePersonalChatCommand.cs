using Application.Interfaces;
using Application.Messages.UseCases.Dto;
using Application.Messages.UseCases.Queries;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PersonalChats.UseCases.Commands
{
    public class CreatePersonalChatCommand : IRequest<PersonalChatDto>
    {
        public int UserOneId { get; set; }
        public int UserTwoId { get; set; }

    }

    public class CreatePersonalChatCommandHandler : IRequestHandler<CreatePersonalChatCommand, PersonalChatDto>
    {
        private readonly IPersonalChatRepository _personalChatRepository;
        private readonly IMediator _mediator;

        public CreatePersonalChatCommandHandler(IPersonalChatRepository personalChatRepository, IMediator mediator)
        {
            _personalChatRepository = personalChatRepository;
            _mediator = mediator;
        }

        public async Task<PersonalChatDto> Handle(CreatePersonalChatCommand request, CancellationToken cancellationToken)
        {
            var existingChatId = await _personalChatRepository.ExistChat(request.UserOneId, request.UserTwoId);

            if (existingChatId == 0)
            {
                var chat = new PersonalChat
                {
                    UserOneId = request.UserOneId,
                    UserTwoId = request.UserTwoId,
                };
                var createChat = await _personalChatRepository.CreatePersonalChat(chat);
                return new PersonalChatDto
                {
                    Id = createChat.Id,
                    UserOneId = createChat.UserOneId,
                    UserTwoId = createChat.UserTwoId,
                    CreatedAt = createChat.CreatedAt,
                    Messages = new List<MessageDto>()
                };
            }

            var dbChat = await _personalChatRepository.GetPersonalChatById(existingChatId);
            var messages = await _mediator.Send(new GetMessagesByPersonalChatIdQuery
            {
                PersonalId = existingChatId
            }, cancellationToken);

            return new PersonalChatDto
            {
                Id = dbChat.Id,
                UserOneId = dbChat.UserOneId,
                UserTwoId = dbChat.UserTwoId,
                CreatedAt = dbChat.CreatedAt,
                IsArchived = dbChat.IsArchived,
                IsBlocked = dbChat.IsBlocked,
                BlockedByUserId = dbChat.BlockedByUserId,
                LastMessageText = dbChat.LastMessageText,
                LastMessageTime = dbChat.LastMessageTime,
                Messages = messages
            };
        }
    }
}
