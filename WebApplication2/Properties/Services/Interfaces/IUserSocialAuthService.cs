using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IUserSocialAuthService
    {
        Task<List<UserSocialAuth>> GetUserSocialAuthsAsync();
        Task<UserSocialAuth?> GetUserSocialAuthByIdAsync(int id);
        Task<UserSocialAuth> CreateUserSocialAuthAsync(UserSocialAuth auth);
        Task<bool> UpdateUserSocialAuthAsync(int id, UserSocialAuth auth);
        Task<bool> DeleteUserSocialAuthAsync(int id);
        Task<bool> UserSocialAuthExistsAsync(int id);
    }
}
