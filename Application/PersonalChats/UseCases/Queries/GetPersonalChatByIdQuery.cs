using Application.Interfaces;
using Application.Messages.UseCases.Dto;
using Application.Messages.UseCases.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PersonalChats.UseCases.Queries
{
    public class GetPersonalChatByIdQuery : IRequest<PersonalChatDto>
    {
        public int PersonalId { get; set; }
    }

    public class GetPersonalChatByIdQueryHandler : IRequestHandler<GetPersonalChatByIdQuery, PersonalChatDto>
    {
        private readonly IPersonalChatRepository _personalChatRepository;
        private readonly IMediator _mediator; 
        public GetPersonalChatByIdQueryHandler(IPersonalChatRepository personalChatRepository, IMediator mediator)
        {
            _personalChatRepository = personalChatRepository;
            _mediator = mediator;
        }
        public async Task<PersonalChatDto> Handle(GetPersonalChatByIdQuery request, CancellationToken cancellationToken)
        {
            var chat = await _personalChatRepository.GetPersonalChatById(request.PersonalId);
            if (chat == null)
                return null;

            var messages = await _mediator.Send(new GetMessagesByPersonalChatIdQuery { PersonalId = chat.Id});

            return new PersonalChatDto
            {
                Id = chat.Id,
                CreatedAt = chat.CreatedAt,
                IsArchived  = chat.IsArchived,
                BlockedByUserId = chat.BlockedByUserId,
                IsBlocked = chat.IsBlocked,
                LastMessageText = chat.LastMessageText,
                LastMessageTime = chat.LastMessageTime,
                Messages = messages,
                UserOneId = chat.UserOneId,
                UserTwoId = chat.UserTwoId, 
            };
        }
    }
}
