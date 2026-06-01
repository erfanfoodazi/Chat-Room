using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.GroupChats.UseCases.Commands
{
    public class AddUserToGroupCommand : IRequest<bool>
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }

    public class AddUserToGroupCommandHandler : IRequestHandler<AddUserToGroupCommand, bool>
    {
        private readonly IGroupChatRepository _groupChatRepository;
        private readonly IUserRepository _userRepository;
        public AddUserToGroupCommandHandler(IGroupChatRepository groupChatRepository, IUserRepository userRepository)
        {
            _groupChatRepository = groupChatRepository;
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(AddUserToGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupChatRepository.GetGroupChatByGroupId(request.GroupId);

            if (group == null) 
                return false;

            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null) return false;

            return await _groupChatRepository.AddUserToGroupId(group.Id, user);
        }
    }
}
