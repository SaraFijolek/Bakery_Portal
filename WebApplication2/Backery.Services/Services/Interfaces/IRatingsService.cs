using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IRatingsService
    {
        Task<IEnumerable<RatingDto>> GetRatingsAsync();
        Task<RatingDto?> GetRatingByIdAsync(int id);
        Task<RatingDto> CreateRatingAsync(CreateRatingDto createRatingDto);
        Task<RatingDto> UpdateRatingAsync(int id, UpdateRatingDto updateRatingDto);
        Task<bool> DeleteRatingAsync(int id);
        Task<bool> RatingExistsAsync(int id);
    }
}
