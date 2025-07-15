using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IRatingsService
    {
        Task<IEnumerable<Rating>>GetRatingsAsync();
        Task<Rating?>GetRatingByIdAsync(int id);
        Task<Rating>CreateRatingAsync(Rating rating);
        Task<Rating>UpdateRatingAsync(int id ,Rating rating);
        Task<bool>DeleteRatingAsync(int id);
        Task<bool>RatingExistsAsync(int id);
    }
}
