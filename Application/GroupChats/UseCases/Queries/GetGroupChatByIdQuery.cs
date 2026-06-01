using Application.GroupChats.UseCases.DTOs;
using Application.Interfaces;
using Application.Messages.UseCases.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Application.GroupChats.UseCases.Queries
{
    public class GetGroupChatByIdQuery : IRequest<GroupDto>
    {
        public int GroupId { get; set; }
    }

    public class GetGroupChatByIdQueryHandler : IRequestHandler<GetGroupChatByIdQuery, GroupDto>
    {
        private readonly IGroupChatRepository _groupChatRepository;
        private readonly IMediator _mediator;
        public GetGroupChatByIdQueryHandler(IGroupChatRepository groupChatRepository, IMediator mediator)
        {
            _groupChatRepository = groupChatRepository;
            _mediator = mediator;
        }
        public async Task<GroupDto> Handle(GetGroupChatByIdQuery request, CancellationToken cancellationToken)
        {
            var group = await _groupChatRepository.GetGroupChatByGroupId(request.GroupId);

            if (group == null)
                return null;

            var messages = await _mediator.Send(new GetMessagesByGroupChatIdQuery
            { GroupId = group.Id });

            var members = group.Members.Select(m => new GroupMemberDto
            {
                GroupChatId = m.GroupChatId,
                UserId = m.UserId,
                Role = m.Role,
                JoinedAt = m.JoinedAt,
                LastReadTime = m.LastReadTime,
            }).ToList();

            return new GroupDto
            {
                Id = group.Id,
                CreatedAt = group.CreatedAt,
                Description = group.Description,
                IsPublic = group.IsPublic,
                Members = members,
                Messages = messages,
                Name = group.Name,
                OwnerId = group.OwnerId,
                ProfilePictureUrl = group.ProfilePictureUrl,
            };
        }
    }
}
