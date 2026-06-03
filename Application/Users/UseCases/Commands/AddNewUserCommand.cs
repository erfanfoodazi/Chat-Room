using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.UseCases.Commands
{
    public class AddNewUserCommand : IRequest<int>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }

    public class AddNewUserCommandHandler : IRequestHandler<AddNewUserCommand, int>
    {
        private readonly IUserRepository _userRepository;
        public AddNewUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> Handle(AddNewUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User()
            {
                FullName = request.FullName,
                Bio = request.Bio,
                Email = request.Email,
                UserName = request.UserName,
                ProfilePictureUrl = request.ProfilePictureUrl,
                CreatedAt = DateTime.UtcNow,
                
            };
            if (!IsPasswordStrong(request.Password))
            {
                throw new Exception("Password is so easy");
            }

            var result = await _userRepository.AddUserAsync(user,request.Password);

            return result.Id;
        }
        private bool IsPasswordStrong(string password)
        {
            if (password.Length < 6)
                return false;

            bool hasUpper = false, hasLower = false, hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
            }

            return hasUpper && hasLower && hasDigit;
        }
    }
}
