using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
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

        public async Task<IEnumerable<Rating>> GetRatingsAsync()
        {
            return await _context.Ratings
                .Include(r => r.FromUser)
                .Include(r => r.ToUser)
                .ToListAsync();
        }

        public async Task<Rating?> GetRatingByIdAsync(int id)
        {
            return await _context.Ratings
                .Include(r => r.FromUser)
                .Include(r => r.ToUser)
                .FirstOrDefaultAsync(r => r.RatingId == id);
        }

        public async Task<Rating> CreateRatingAsync(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            return rating;
        }

        public async Task<Rating> UpdateRatingAsync(int id, Rating rating)
        {
            if (id != rating.RatingId)
                throw new ArgumentException("Rating ID mismatch");

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

            return rating;
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
