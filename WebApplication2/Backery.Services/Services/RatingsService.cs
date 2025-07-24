using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class RatingsService : IRatingsService
    {
        private readonly AppDbContext _context;

        public RatingsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RatingDto>> GetRatingsAsync()
        {
            var ratings = await _context.Ratings
                .Include(r => r.FromUser)
                .Include(r => r.ToUser)
                .ToListAsync();

            return ratings.Select(r => new RatingDto
            {
                RatingId = r.RatingId,
                FromUserId = r.FromUserId,
                ToUserId = r.ToUserId,
                Score = r.Score,
                CreatedAt = r.CreatedAt,
                FromUserName = r.FromUser?.Name ?? string.Empty,
                ToUserName = r.ToUser?.Name ?? string.Empty
            });
        }

        public async Task<RatingDto?> GetRatingByIdAsync(int id)
        {
            var rating = await _context.Ratings
                .Include(r => r.FromUser)
                .Include(r => r.ToUser)
                .FirstOrDefaultAsync(r => r.RatingId == id);

            if (rating == null)
                return null;

            return new RatingDto
            {
                RatingId = rating.RatingId,
                FromUserId = rating.FromUserId,
                ToUserId = rating.ToUserId,
                Score = rating.Score,
                CreatedAt = rating.CreatedAt,
                FromUserName = rating.FromUser?.Name ?? string.Empty,
                ToUserName = rating.ToUser?.Name ?? string.Empty
            };
        }

        public async Task<RatingDto> CreateRatingAsync(CreateRatingDto createRatingDto)
        {
            var rating = new Rating
            {
                FromUserId = createRatingDto.FromUserId,
                ToUserId = createRatingDto.ToUserId,
                Score = createRatingDto.Score,
                CreatedAt = DateTime.Now
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            // Load the created rating with navigation properties
            var createdRating = await _context.Ratings
                .Include(r => r.FromUser)
                .Include(r => r.ToUser)
                .FirstOrDefaultAsync(r => r.RatingId == rating.RatingId);

            return new RatingDto
            {
                RatingId = createdRating.RatingId,
                FromUserId = createdRating.FromUserId,
                ToUserId = createdRating.ToUserId,
                Score = createdRating.Score,
                CreatedAt = createdRating.CreatedAt,
                FromUserName = createdRating.FromUser?.Name ?? string.Empty,
                ToUserName = createdRating.ToUser?.Name ?? string.Empty
            };
        }

        public async Task<RatingDto> UpdateRatingAsync(int id, UpdateRatingDto updateRatingDto)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
                throw new KeyNotFoundException($"Rating with ID {id} not found");

            rating.FromUserId = updateRatingDto.FromUserId;
            rating.ToUserId = updateRatingDto.ToUserId;
            rating.Score = updateRatingDto.Score;

            _context.Entry(rating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RatingExistsAsync(id))
                    throw new KeyNotFoundException($"Rating with ID {id} not found");
                throw;
            }

            // Load the updated rating with navigation properties
            var updatedRating = await _context.Ratings
                .Include(r => r.FromUser)
                .Include(r => r.ToUser)
                .FirstOrDefaultAsync(r => r.RatingId == id);

            return new RatingDto
            {
                RatingId = updatedRating.RatingId,
                FromUserId = updatedRating.FromUserId,
                ToUserId = updatedRating.ToUserId,
                Score = updatedRating.Score,
                CreatedAt = updatedRating.CreatedAt,
                FromUserName = updatedRating.FromUser?.Name ?? string.Empty,
                ToUserName = updatedRating.ToUser?.Name ?? string.Empty
            };
        }

        public async Task<bool> DeleteRatingAsync(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
                return false;

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RatingExistsAsync(int id)
        {
            return await _context.Ratings.AnyAsync(e => e.RatingId == id);
        }
    }
}
