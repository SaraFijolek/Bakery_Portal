using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IUserModerationsService
    {
        Task<List<UserModerationDto>> GetUserModerationsAsync();
        Task<UserModerationDto?> GetUserModerationByIdAsync(int id);
        Task<UserModerationDto> CreateUserModerationAsync(CreateUserModerationDto createDto);
        Task<bool> UpdateUserModerationAsync(int id, UpdateUserModerationDto updateDto);
        Task<bool> DeleteUserModerationAsync(int id);
        Task<bool> UserModerationExistsAsync(int id);

        Task<List<UserModerationListDto>> GetUserModerationsListAsync();
        Task<List<UserModerationDto>> GetUserModerationsByUserIdAsync(int userId);
        Task<List<UserModerationDto>> GetActiveUserModerationsAsync();
    }
}
