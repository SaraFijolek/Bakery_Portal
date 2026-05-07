using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IUserSocialAuthService
    {
        Task<UserSocialAuthsService<List<UserSocialAuthDto>>> GetUserSocialAuthsAsync();
        Task<UserSocialAuthsService<UserSocialAuthDto>> GetUserSocialAuthByIdAsync(int id);
        Task<UserSocialAuthsService<UserSocialAuthDto>> CreateUserSocialAuthAsync(CreateUserSocialAuthDto createDto);
        Task<UserSocialAuthsService<UserSocialAuthDto>> UpdateUserSocialAuthAsync(int id, UpdateUserSocialAuthDto updateDto);
        Task<UserSocialAuthsService<bool>> DeleteUserSocialAuthAsync(int id);
        Task<bool> UserSocialAuthExistsAsync(int id);
    }
}
