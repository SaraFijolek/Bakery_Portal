using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Properties.Services
{
    public class RatingService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static RatingService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new RatingService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static RatingService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default)
            => new RatingService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    public class RatingsService : IRatingsService
    {
        private readonly AppDbContext _context;

        public RatingsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RatingService<IEnumerable<RatingDto>>> GetRatingsAsync()
        {
            try
            {
                var ratings = await _context.Ratings
                    .Include(r => r.FromUser)
                    .Include(r => r.ToUser)
                    .ToListAsync();

                var ratingDtos = ratings.Select(r => new RatingDto
                {
                    RatingId = r.RatingId,
                    FromUserId = r.FromUserId,
                    ToUserId = r.ToUserId,
                    Score = r.Score,
                    CreatedAt = r.CreatedAt,
                    FromUserName = r.FromUser?.UserName ?? string.Empty,
                    ToUserName = r.ToUser?.UserName ?? string.Empty
                });

                return RatingService<IEnumerable<RatingDto>>.GoodResult(
                    "Ratings retrieved successfully",
                    200,
                    ratingDtos);
            }
            catch (Exception ex)
            {
                return RatingService<IEnumerable<RatingDto>>.BadResult(
                    "Failed to retrieve ratings",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<RatingService<RatingDto>> GetRatingByIdAsync(int id)
        {
            try
            {
                var rating = await _context.Ratings
                    .Include(r => r.FromUser)
                    .Include(r => r.ToUser)
                    .FirstOrDefaultAsync(r => r.RatingId == id);

                if (rating == null)
                {
                    return RatingService<RatingDto>.BadResult(
                        $"Rating with ID {id} not found",
                        404);
                }

                var ratingDto = new RatingDto
                {
                    RatingId = rating.RatingId,
                    FromUserId = rating.FromUserId,
                    ToUserId = rating.ToUserId,
                    Score = rating.Score,
                    CreatedAt = rating.CreatedAt,
                    FromUserName = rating.FromUser?.UserName ?? string.Empty,
                    ToUserName = rating.ToUser?.UserName ?? string.Empty
                };

                return RatingService<RatingDto>.GoodResult(
                    "Rating retrieved successfully",
                    200,
                    ratingDto);
            }
            catch (Exception ex)
            {
                return RatingService<RatingDto>.BadResult(
                    "Failed to retrieve rating",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<RatingService<RatingDto>> CreateRatingAsync(CreateRatingDto createRatingDto)
        {
            try
            {
                // Validate that FromUserId and ToUserId exist
                var fromUserExists = await _context.Users.AnyAsync(u => u.Id == createRatingDto.FromUserId);
                var toUserExists = await _context.Users.AnyAsync(u => u.Id == createRatingDto.ToUserId);

                var errors = new List<string>();
                if (!fromUserExists)
                    errors.Add($"FromUser with ID {createRatingDto.FromUserId} does not exist");
                if (!toUserExists)
                    errors.Add($"ToUser with ID {createRatingDto.ToUserId} does not exist");
                if (createRatingDto.FromUserId == createRatingDto.ToUserId)
                    errors.Add("User cannot rate themselves");

                if (errors.Any())
                {
                    return RatingService<RatingDto>.BadResult(
                        "Validation failed",
                        400,
                        errors);
                }

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

                var ratingDto = new RatingDto
                {
                    RatingId = createdRating.RatingId,
                    FromUserId = createdRating.FromUserId,
                    ToUserId = createdRating.ToUserId,
                    Score = createdRating.Score,
                    CreatedAt = createdRating.CreatedAt,
                    FromUserName = createdRating.FromUser?.UserName ?? string.Empty,
                    ToUserName = createdRating.ToUser?.UserName ?? string.Empty
                };

                return RatingService<RatingDto>.GoodResult(
                    "Rating created successfully",
                    201,
                    ratingDto);
            }
            catch (Exception ex)
            {
                return RatingService<RatingDto>.BadResult(
                    "Failed to create rating",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<RatingService<RatingDto>> UpdateRatingAsync(int id, UpdateRatingDto updateRatingDto)
        {
            try
            {
                var rating = await _context.Ratings.FindAsync(id);
                if (rating == null)
                {
                    return RatingService<RatingDto>.BadResult(
                        $"Rating with ID {id} not found",
                        404);
                }

                // Validate that FromUserId and ToUserId exist
                var fromUserExists = await _context.Users.AnyAsync(u => u.Id == updateRatingDto.FromUserId);
                var toUserExists = await _context.Users.AnyAsync(u => u.Id == updateRatingDto.ToUserId);

                var errors = new List<string>();
                if (!fromUserExists)
                    errors.Add($"FromUser with ID {updateRatingDto.FromUserId} does not exist");
                if (!toUserExists)
                    errors.Add($"ToUser with ID {updateRatingDto.ToUserId} does not exist");
                if (updateRatingDto.FromUserId == updateRatingDto.ToUserId)
                    errors.Add("User cannot rate themselves");

                if (errors.Any())
                {
                    return RatingService<RatingDto>.BadResult(
                        "Validation failed",
                        400,
                        errors);
                }

                rating.FromUserId = updateRatingDto.FromUserId;
                rating.ToUserId = updateRatingDto.ToUserId;
                rating.Score = updateRatingDto.Score;

                _context.Entry(rating).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                // Load the updated rating with navigation properties
                var updatedRating = await _context.Ratings
                    .Include(r => r.FromUser)
                    .Include(r => r.ToUser)
                    .FirstOrDefaultAsync(r => r.RatingId == id);

                var ratingDto = new RatingDto
                {
                    RatingId = updatedRating.RatingId,
                    FromUserId = updatedRating.FromUserId,
                    ToUserId = updatedRating.ToUserId,
                    Score = updatedRating.Score,
                    CreatedAt = updatedRating.CreatedAt,
                    FromUserName = updatedRating.FromUser?.UserName ?? string.Empty,
                    ToUserName = updatedRating.ToUser?.UserName ?? string.Empty
                };

                return RatingService<RatingDto>.GoodResult(
                    "Rating updated successfully",
                    200,
                    ratingDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RatingExistsAsync(id))
                {
                    return RatingService<RatingDto>.BadResult(
                        $"Rating with ID {id} not found",
                        404);
                }

                return RatingService<RatingDto>.BadResult(
                    "Concurrency conflict occurred while updating rating",
                    409);
            }
            catch (Exception ex)
            {
                return RatingService<RatingDto>.BadResult(
                    "Failed to update rating",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<RatingService<bool>> DeleteRatingAsync(int id)
        {
            try
            {
                var rating = await _context.Ratings.FindAsync(id);
                if (rating == null)
                {
                    return RatingService<bool>.BadResult(
                        $"Rating with ID {id} not found",
                        404,
                        data: false);
                }

                _context.Ratings.Remove(rating);
                await _context.SaveChangesAsync();

                return RatingService<bool>.GoodResult(
                    "Rating deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return RatingService<bool>.BadResult(
                    "Failed to delete rating",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<bool> RatingExistsAsync(int id)
        {
            return await _context.Ratings.AnyAsync(e => e.RatingId == id);
        }
    }
}