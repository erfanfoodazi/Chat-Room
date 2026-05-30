using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.UseCases.Commnads
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User()
            {
                Id = request.Id,
                Email = request.Email,
                UserName = request.UserName,
                Bio = request.Bio,
                FullName = request.FullName,
                ProfilePictureUrl = request.ProfilePictureUrl,
            };

            var result = await _userRepository.UpdateUserAsync(user);
            return result;

        }
    }
}
