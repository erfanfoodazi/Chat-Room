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
    public class GetUserByPhoneNumberQuery : IRequest<UserDto>
    {
        public string PhoneNumber { get; set; } = string.Empty; 
    }

    public class GetUserByPhoneNumberQueryHandler : IRequestHandler<GetUserByPhoneNumberQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        public GetUserByPhoneNumberQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserDto> Handle(GetUserByPhoneNumberQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByPhoneNumberAsync(request.PhoneNumber);
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
