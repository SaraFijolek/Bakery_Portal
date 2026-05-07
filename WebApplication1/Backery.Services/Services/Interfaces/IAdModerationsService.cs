using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdModerationsService
    {
        Task<AdModerationService<IEnumerable<AdModerationDto>>> GetAdModerationsAsync();
        Task<AdModerationService<AdModerationDto>> GetAdModerationByIdAsync(int id);
        Task<AdModerationService<AdModerationDto>> CreateAdModerationAsync(CreateAdModerationDto createDto);
        Task<AdModerationService<AdModerationDto>> UpdateAdModerationAsync(int id, UpdateAdModerationDto updateDto);
        Task<AdModerationService<bool>> DeleteAdModerationAsync(int id);
        Task<bool> AdModerationExistsAsync(int id);
    }
}
