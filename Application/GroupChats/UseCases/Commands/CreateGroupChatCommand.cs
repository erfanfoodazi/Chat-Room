using Application.GroupChats.UseCases.DTOs;
using Application.Interfaces;
using Application.Users.UseCases.DTOs;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.GroupChats.UseCases.Commands
{
    public class CreateGroupChatCommand : IRequest<GroupDto>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public string? ProfilePictureUrl { get; set; }
        public int OwnerId { get; set; }
        public bool IsPublic { get; set; } 

    }

    public class CreateGroupChatCommandHandler : IRequestHandler<CreateGroupChatCommand, GroupDto>
    {
        private readonly IGroupChatRepository _groupChatRepository;
        private readonly IUserRepository _userRepository;
        public CreateGroupChatCommandHandler(IGroupChatRepository groupChatRepository, IUserRepository userRepository)
        {
            _groupChatRepository = groupChatRepository;
            _userRepository = userRepository;
        }
        public async Task<GroupDto> Handle(CreateGroupChatCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.OwnerId);

            if (user == null)
                return null;

            var group = new GroupChat
            {
                Name = request.Name,
                Description = request.Description,
                ProfilePictureUrl = request.ProfilePictureUrl,
                OwnerId = request.OwnerId,
                CreatedAt = DateTime.UtcNow,
                IsPublic = request.IsPublic,
                Owner = user,
            };

            var createGroup = await _groupChatRepository.CreateGroupChat(group, user);

            var owner = new GroupMemberDto
            {
                GroupChatId = createGroup.Id,
                JoinedAt = DateTime.UtcNow,
                UserId = user.Id,
                Role = GroupRole.Owner,
            };
            var members = new List<GroupMemberDto>();
            members.Add(owner);

            return new GroupDto
            {
                Id = createGroup.Id,
                CreatedAt = createGroup.CreatedAt,
                Description = createGroup.Description,
                IsPublic = createGroup.IsPublic,
                Members = members,
                Name = createGroup.Name,
                OwnerId = user.Id,
                ProfilePictureUrl = createGroup.ProfilePictureUrl
            };
        }
    }
}
