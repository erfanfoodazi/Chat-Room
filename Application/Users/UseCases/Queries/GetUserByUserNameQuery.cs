using Application.Interfaces;
using Application.Users.UseCases.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.UseCases.Queries
{
    public class GetUserByUserNameQuery : IRequest<UserDto>
    {
        public string UserName { get; set; } = string.Empty;
    }

    public class GetUserByUserNameQueryHandler : IRequestHandler<GetUserByUserNameQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        public GetUserByUserNameQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserDto> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.UserName);
            if (user == null)
                return null;

            return new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                CreatedAt = user.CreatedAt,
                LastSeen = user.LastSeen,
            };
        }
    }
}
