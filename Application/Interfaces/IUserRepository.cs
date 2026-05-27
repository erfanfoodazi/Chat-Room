using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> UpdateUserAsync(User user);
       
    }
}
