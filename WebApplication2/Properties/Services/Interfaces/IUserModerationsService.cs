using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IUserModerationsService
    {
        Task<List<UserModeration>> GetUserModerationsAsync();
        Task<UserModeration?>GetUserModerationByIdAsync(int id);
        Task<UserModeration>CreateUserModerationAsync(UserModeration moderation);
        Task<bool>UpdateUserModerationAsync(int id, UserModeration moderation);
        Task<bool>DeleteUserModerationAsync(int id);
        Task<bool>UserModerationExistsAsync(int id);
    }
}
