using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdModerationsService
    {
        Task<IEnumerable<AdModerationDto>> GetAdModerationsAsync();
        Task<AdModerationDto?> GetAdModerationByIdAsync(int id);
        Task<AdModerationDto> CreateAdModerationAsync(CreateAdModerationDto createDto);
        Task<AdModerationDto> UpdateAdModerationAsync(int id, UpdateAdModerationDto updateDto);
        Task<bool> DeleteAdModerationAsync(int id);
        Task<bool> AdModerationExistsAsync(int id);
    }
}
