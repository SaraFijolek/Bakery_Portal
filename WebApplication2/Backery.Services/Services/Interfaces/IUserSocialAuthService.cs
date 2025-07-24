using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IUserSocialAuthService
    {
        Task<List<UserSocialAuthDto>> GetUserSocialAuthsAsync();
        Task<UserSocialAuthDto> GetUserSocialAuthByIdAsync(int id);
        Task<UserSocialAuthDto> CreateUserSocialAuthAsync(CreateUserSocialAuthDto createDto);
        Task<bool> UpdateUserSocialAuthAsync(int id, UpdateUserSocialAuthDto updateDto);
        Task<bool> DeleteUserSocialAuthAsync(int id);
        Task<bool> UserSocialAuthExistsAsync(int id);
    }
}
