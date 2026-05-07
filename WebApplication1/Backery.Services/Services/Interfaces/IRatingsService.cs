using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IRatingsService
    {
        Task<RatingService<IEnumerable<RatingDto>>> GetRatingsAsync();
        Task<RatingService<RatingDto>> GetRatingByIdAsync(int id);
        Task<RatingService<RatingDto>> CreateRatingAsync(CreateRatingDto createRatingDto);
        Task<RatingService<RatingDto>> UpdateRatingAsync(int id, UpdateRatingDto updateRatingDto);
        Task<RatingService<bool>> DeleteRatingAsync(int id);
        Task<bool> RatingExistsAsync(int id);
    }
}
