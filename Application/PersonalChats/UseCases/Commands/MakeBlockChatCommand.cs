using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PersonalChats.UseCases.Commands
{
    public class MakeBlockChatCommand : IRequest<bool>
    {
        public int PersonalChatId { get; set; }
        public int BlockerId { get; set; }
    }

    public class MakeBlockChatCommandHandler : IRequestHandler<MakeBlockChatCommand, bool>
    {
        private readonly IPersonalChatRepository _personalChatRepository;
        public MakeBlockChatCommandHandler(IPersonalChatRepository personalChatRepository)
        {
            _personalChatRepository = personalChatRepository;
        }
        public async Task<bool> Handle(MakeBlockChatCommand request, CancellationToken cancellationToken)
        {
            return await _personalChatRepository.MakeBlockChat(request.PersonalChatId, request.BlockerId);
        }
    }
}
