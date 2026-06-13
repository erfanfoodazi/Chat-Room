using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.GroupChats.UseCases.Commands
{
    public class DeleteGroupChatCommand : IRequest<bool>
    {
        public int GroupId { get; set; }
        public int RequestedByUserId { get; set; }
    }

    public class DeleteGroupChatCommandHandler : IRequestHandler<DeleteGroupChatCommand, bool>
    {
        private readonly IGroupChatRepository _groupChatRepository;
        public DeleteGroupChatCommandHandler(IGroupChatRepository groupChatRepository)
        {
            _groupChatRepository = groupChatRepository; 
        }
        public async Task<bool> Handle(DeleteGroupChatCommand request, CancellationToken cancellationToken)
        {
            var role = await _groupChatRepository.GetGroupMemberRole(request.GroupId, request.RequestedByUserId);
            if (role != GroupRole.Owner)
                return false;

            return await _groupChatRepository.DeleteGroupChat(request.GroupId);
        }
    }
}
