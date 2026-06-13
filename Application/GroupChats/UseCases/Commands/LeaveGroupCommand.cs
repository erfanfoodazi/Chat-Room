using Application.Interfaces;
using MediatR;

namespace Application.GroupChats.UseCases.Commands
{
    public class LeaveGroupCommand : IRequest<bool>
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }

    public class LeaveGroupCommandHandler : IRequestHandler<LeaveGroupCommand, bool>
    {
        private readonly IGroupChatRepository _groupChatRepository;
        public LeaveGroupCommandHandler(IGroupChatRepository groupChatRepository)
        {
            _groupChatRepository = groupChatRepository;
        }
        public async Task<bool> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
        {
            return await _groupChatRepository.RemoveUserFromGroup(request.GroupId, request.UserId);
        }
    }
}
