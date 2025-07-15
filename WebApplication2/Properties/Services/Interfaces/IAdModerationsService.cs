using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdModerationsService
    {
        Task<IEnumerable<AdModeration>>GetAdModerationsAsync();
        Task<AdModeration?>GetAdModerationByIdAsync(int id);
        Task<AdModeration>CreateAdModerationAsync(AdModeration moderation);
        Task<AdModeration>UpdateAdModerationAsync(int id,AdModeration moderation);
        Task<bool>DeleteAdModerationAsync(int id);
        Task<bool>AdModerationExistsAsync(int id);
    }
}
