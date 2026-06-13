using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.GroupChats.UseCases.Commands
{
    public class UpdateGroupChatCommand : IRequest<bool>
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public int RequestedByUserId { get; set; }
    }

    public class UpdateGroupChatCommandHandler : IRequestHandler<UpdateGroupChatCommand, bool>
    {
        private readonly IGroupChatRepository _groupChatRepository;
        public UpdateGroupChatCommandHandler(IGroupChatRepository groupChatRepository)
        {
            _groupChatRepository = groupChatRepository;
        }
        public async Task<bool> Handle(UpdateGroupChatCommand request, CancellationToken cancellationToken)
        {
            var role = await _groupChatRepository.GetGroupMemberRole(request.GroupId, request.RequestedByUserId);
            if (role != GroupRole.Owner && role != GroupRole.Admin)
                return false;

            var group = await _groupChatRepository.GetGroupChatByGroupId(request.GroupId);
            if (group == null)
                return false;

            group.Name = request.Name;
            if (request.Description != string.Empty)
                group.Description = request.Description;
            if (request.ProfilePictureUrl != string.Empty)
                group.ProfilePictureUrl = request.ProfilePictureUrl;

            return await _groupChatRepository.UpdateGroupChat(group);
        }
    }
}
