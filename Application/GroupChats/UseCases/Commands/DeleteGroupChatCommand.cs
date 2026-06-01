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
            return await _groupChatRepository.DeleteGroupChat(request.GroupId);
        }
    }
}
