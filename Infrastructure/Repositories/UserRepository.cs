using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class UserRepsitory : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserRepsitory> _logger;

        public UserRepsitory(UserManager<User> userManager, ILogger<UserRepsitory> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<User> AddUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create user: {Errors}", errors);
                throw new Exception($"Failed to create user: {errors}");
            }
            return user;
        }

        public async Task<bool> ChangeUserPassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for password change", userId);
                return false;
            }
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for deletion", userId);
                return false;
            }
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return null;
            }
            return user;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return null;
            }
            return user;
        }

        public async Task<User> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user == null)
            {
                _logger.LogWarning("User with phone {PhoneNumber} not found", phoneNumber);
                return null;
            }
            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("User with username {Username} not found", username);
                return null;
            }
            return user;
        }

        public async Task<bool> MakeUserOnlineOrOffline(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return false;
            }
            if (user.IsOnline)
            {
                user.SetOffline();
            }
            else 
            {
                user.SetOnline();
            }
            
            await _userManager.UpdateAsync(user);
            return user.IsOnline;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (existingUser == null)
            {
                _logger.LogError("User {UserId} not found for update", user.Id);
                return false;
            }

            existingUser.FullName = user.FullName;
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.ProfilePictureUrl = user.ProfilePictureUrl;

            var result = await _userManager.UpdateAsync(existingUser);
            return result.Succeeded;
        }
    }
}