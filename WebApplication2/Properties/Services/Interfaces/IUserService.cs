using System.Collections.Generic;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);

    }
}
