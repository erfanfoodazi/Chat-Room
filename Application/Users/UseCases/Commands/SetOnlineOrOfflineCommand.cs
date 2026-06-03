using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.UseCases.Commands
{
    public class SetOnlineOrOfflineCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }

    public class SetOnlineOrOfflineCommandHandler : IRequestHandler<SetOnlineOrOfflineCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        public SetOnlineOrOfflineCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(SetOnlineOrOfflineCommand request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.MakeUserOnlineOrOffline(request.UserId);
            return result;
        }
    }
}
