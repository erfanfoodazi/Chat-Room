using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class UserRepsitory : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepsitory> _logger;
        public UserRepsitory(AppDbContext context,ILogger<UserRepsitory> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);


            if (user == null)
            {
                _logger.LogError($"Can't find any user with this id {userId}");
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogError("User not found");
                return null;
            }
            return user;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogError("User not found");
                return null;
            }
            return user;
        }

        public async Task<User> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user == null)
            {
                _logger.LogError("User not found");
                return null;
            }
            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                _logger.LogError("user not found");
                return null;
            }
            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null)
            {
                _logger.LogError("User not found");
                return false;
            }

            existingUser.FullName = user.FullName;
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.ProfilePictureUrl = user.ProfilePictureUrl;
            existingUser.Bio = user.Bio; // اگر داری
            existingUser.UpdatedAt = DateTime.UtcNow; // اگر این فیلد را اضافه کرده باشی

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
