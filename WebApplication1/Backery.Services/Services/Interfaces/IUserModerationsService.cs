using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IUserModerationsService
    {
        Task<UserModerationService<List<UserModerationDto>>> GetUserModerationsAsync();
        Task<UserModerationService<List<UserModerationListDto>>> GetUserModerationsListAsync();
        Task<UserModerationService<UserModerationDto>> GetUserModerationByIdAsync(int id);
        Task<UserModerationService<List<UserModerationDto>>> GetUserModerationsByUserIdAsync(string userId);
        Task<UserModerationService<List<UserModerationDto>>> GetActiveUserModerationsAsync();
        Task<UserModerationService<UserModerationDto>> CreateUserModerationAsync(CreateUserModerationDto createDto);
        Task<UserModerationService<bool>> UpdateUserModerationAsync(int id, UpdateUserModerationDto updateDto);
        Task<UserModerationService<bool>> DeleteUserModerationAsync(int id);
    }
}
