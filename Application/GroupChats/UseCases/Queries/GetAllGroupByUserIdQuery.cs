using Application.GroupChats.UseCases.DTOs;
using Application.Interfaces;
using Application.Messages.UseCases.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.GroupChats.UseCases.Queries
{
    public class GetAllGroupByUserIdQuery : IRequest<List<GroupDto>>
    {
        public int UserId { get; set; }
    }

    public class GetAllGroupByUserIdQueryHandler : IRequestHandler<GetAllGroupByUserIdQuery, List<GroupDto>>
    {
        private readonly IGroupChatRepository _groupChatRepository;
        private readonly IMediator _mediator;
        public GetAllGroupByUserIdQueryHandler(IGroupChatRepository groupChatRepository, IMediator mediator)
        {
            _groupChatRepository = groupChatRepository;
            _mediator = mediator;
        }
        public async Task<List<GroupDto>> Handle(GetAllGroupByUserIdQuery request, CancellationToken cancellationToken)
        {
            var groups = await _groupChatRepository.GetAllGroupChatByUserId(request.UserId);
            if (groups == null || !groups.Any())
                return new List<GroupDto>();

            var result = new List<GroupDto>();
            foreach (var group in groups)
            {
                var messages = await _mediator.Send(new GetMessagesByGroupChatIdQuery
                {
                    GroupId = group.Id
                }, cancellationToken);

                var members = group.Members.Select(m => new GroupMemberDto
                {
                    GroupChatId = m.GroupChatId,
                    UserId = m.UserId,
                    Role = m.Role,
                    JoinedAt = m.JoinedAt,
                    LastReadTime = m.LastReadTime,
                }).ToList();

                result.Add(new GroupDto
                {
                    Id = group.Id,
                    CreatedAt = group.CreatedAt,
                    Description = group.Description,
                    IsPublic = group.IsPublic,
                    Name = group.Name,
                    OwnerId = group.OwnerId,
                    ProfilePictureUrl = group.ProfilePictureUrl,
                    Members = members,
                    Messages = messages,
                });
            }
            return result;
        }
    }
}
