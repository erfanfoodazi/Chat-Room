using Application.Interfaces;
using Application.Messages.UseCases.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PersonalChats.UseCases.Queries
{
    public class GetPersonalChatsByUserIdQuery : IRequest<List<PersonalChatDto>>
    {
        public int UserId { get; set; }
    }

    public class GetPersonalChatsByUserIdQueryHandler : IRequestHandler<GetPersonalChatsByUserIdQuery, List<PersonalChatDto>>
    {
        private readonly IPersonalChatRepository _personalChatRepository;
        private readonly IMediator _mediator;
        public GetPersonalChatsByUserIdQueryHandler(IPersonalChatRepository personalChatRepository, IMediator mediator)
        {
            _personalChatRepository = personalChatRepository;
            _mediator = mediator;
        }
        public async Task<List<PersonalChatDto>> Handle(GetPersonalChatsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var chats = await _personalChatRepository.GetPersonalChatsByUserId(request.UserId);
            if (chats == null || !chats.Any())
                return new List<PersonalChatDto>();

            var result = new List<PersonalChatDto>();
            foreach (var chat in chats)
            {
                var messages = await _mediator.Send(new GetMessagesByPersonalChatIdQuery
                {
                    PersonalId = chat.Id
                }, cancellationToken);

                result.Add(new PersonalChatDto
                {
                    Id = chat.Id,
                    CreatedAt = chat.CreatedAt,
                    IsArchived = chat.IsArchived,
                    IsBlocked = chat.IsBlocked,
                    BlockedByUserId = chat.BlockedByUserId,
                    LastMessageText = chat.LastMessageText,
                    LastMessageTime = chat.LastMessageTime,
                    UserOneId = chat.UserOneId,
                    UserTwoId = chat.UserTwoId,
                    Messages = messages
                });
            }
            return result;
        }
    }
}
