using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PersonalChats.UseCases.Commands
{
    public class DeletePersonalChatCommand : IRequest<bool>
    {
        public int PersonalChatId { get; set; }
    }

    public class DeletePersonalChatCommandHandler : IRequestHandler<DeletePersonalChatCommand, bool>
    {
        private readonly IPersonalChatRepository _personalChatRepository;
        public DeletePersonalChatCommandHandler(IPersonalChatRepository personalChatRepository)
        {
            _personalChatRepository = personalChatRepository;
        }
        public async Task<bool> Handle(DeletePersonalChatCommand request, CancellationToken cancellationToken)
        {
            return await _personalChatRepository.DeletePesonalChat(request.PersonalChatId);
        }
    }
}
