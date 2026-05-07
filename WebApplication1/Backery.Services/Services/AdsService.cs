using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Backery.Services.Services;
using WebApplication2.DTO;
using WebApplication2.Models;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static AdService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new AdService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static AdService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default)
            =>
            new AdService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    public class AdsService : IAdsService
    {
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        public AdsService(AppDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<AdService<IEnumerable<AdResponseDto>>> GetAllAdsAsync()
        {
            try
            {
                var ads = await _context.Ads
                    .Include(a => a.User)
                    .Include(a => a.Subcategory)
                    .ToListAsync();

                var adResponseDtos = ads.Select(a => new AdResponseDto
                {
                    AdId = a.AdId,
                    UserId = a.UserId,
                    SubcategoryId = a.SubcategoryId,
                    Title = a.Title,
                    Description = a.Description,
                    Price = a.Price,
                    Location = a.Location,
                    CreatedAt = a.CreatedAt,
                    ExpiresAt = a.ExpiresAt,
                    Status = a.Status,
                    User = a.User == null ? null : new UserDto
                    {
                        UserId = a.UserId,
                        Username = a.User.UserName
                        // map other needed fields
                    },
                    Subcategory = a.Subcategory == null ? null : new SubcategoryDto
                    {
                        SubcategoryId = a.Subcategory.SubcategoryId,
                        Name = a.Subcategory.Name
                        // map other needed fields
                    }
                });

                return AdService<IEnumerable<AdResponseDto>>.GoodResult(
                    "Ads retrieved successfully",
                    200,
                    adResponseDtos);
            }
            catch (Exception ex)
            {
                return AdService<IEnumerable<AdResponseDto>>.BadResult(
                    "Failed to retrieve ads",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdService<AdResponseDto>> GetAdByIdAsync(int id)
        {
            try
            {
                var a = await _context.Ads
                    .Include(ad => ad.User)
                    .Include(ad => ad.Subcategory)
                    .FirstOrDefaultAsync(ad => ad.AdId == id);

                if (a == null)
                {
                    return AdService<AdResponseDto>.BadResult(
                        $"Ad with ID {id} not found",
                        404);
                }

                var adResponseDto = new AdResponseDto
                {
                    AdId = a.AdId,
                    UserId = a.UserId,
                    SubcategoryId = a.SubcategoryId,
                    Title = a.Title,
                    Description = a.Description,
                    Price = a.Price,
                    Location = a.Location,
                    CreatedAt = a.CreatedAt,
                    ExpiresAt = a.ExpiresAt,
                    Status = a.Status,
                    User = a.User == null ? null : new UserDto
                    {
                        UserId = a.UserId,
                        Username = a.User.UserName
                        // map other needed fields
                    },
                    Subcategory = a.Subcategory == null ? null : new SubcategoryDto
                    {
                        SubcategoryId = a.Subcategory.SubcategoryId,
                        Name = a.Subcategory.Name
                        // map other needed fields
                    }
                };

                return AdService<AdResponseDto>.GoodResult(
                    "Ad retrieved successfully",
                    200,
                    adResponseDto);
            }
            catch (Exception ex)
            {
                return AdService<AdResponseDto>.BadResult(
                    "Failed to retrieve ad",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdService<AdResponseDto>> CreateAdAsync(AdCreateDto dto)
        {
            try
            {
                var entity = new Ad
                {
                    UserId = dto.UserId,
                    SubcategoryId = dto.SubcategoryId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Price = dto.Price,
                    Location = dto.Location,
                    ExpiresAt = dto.ExpiresAt,
                    Status = dto.Status
                };

                _context.Ads.Add(entity);
                await _context.SaveChangesAsync();
                await NotifyUsersAsync(entity);

                await _context.Entry(entity).Reference(a => a.User).LoadAsync();
                await _context.Entry(entity).Reference(a => a.Subcategory).LoadAsync();

                var adResponseDto = new AdResponseDto
                {
                    AdId = entity.AdId,
                    UserId = entity.UserId,
                    SubcategoryId = entity.SubcategoryId,
                    Title = entity.Title,
                    Description = entity.Description,
                    Price = entity.Price,
                    Location = entity.Location,
                    CreatedAt = entity.CreatedAt,
                    ExpiresAt = entity.ExpiresAt,
                    Status = entity.Status,
                    User = entity.User == null ? null : new UserDto
                    {
                        UserId = entity.UserId,
                        Username = entity.User.UserName
                        // map other needed fields
                    },
                    Subcategory = entity.Subcategory == null ? null : new SubcategoryDto
                    {
                        SubcategoryId = entity.Subcategory.SubcategoryId,
                        Name = entity.Subcategory.Name
                        // map other needed fields
                    }
                };

                return AdService<AdResponseDto>.GoodResult(
                    "Ad created successfully",
                    201,
                    adResponseDto);
            }
            catch (Exception ex)
            {
                return AdService<AdResponseDto>.BadResult(
                    "Failed to create ad",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<AdService<bool>> UpdateAdAsync(AdUpdateDto dto)
        {
            try
            {
                var exists = await _context.Ads.AnyAsync(a => a.AdId == dto.AdId);
                if (!exists)
                {
                    return AdService<bool>.BadResult(
                        $"Ad with ID {dto.AdId} not found",
                        404,
                        data: false);
                }

                var entity = new Ad
                {
                    AdId = dto.AdId,
                    UserId = dto.UserId,
                    SubcategoryId = dto.SubcategoryId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Price = dto.Price,
                    Location = dto.Location,
                    ExpiresAt = dto.ExpiresAt,
                    Status = dto.Status
                    // CreatedAt left unchanged to preserve original timestamp
                };

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return AdService<bool>.GoodResult(
                    "Ad updated successfully",
                    200,
                    true);
            }
            catch (DbUpdateConcurrencyException)
            {
                return AdService<bool>.BadResult(
                    "Concurrency error occurred while updating ad",
                    409,
                    new List<string> { "The ad was modified by another process" },
                    false);
            }
            catch (Exception ex)
            {
                return AdService<bool>.BadResult(
                    "Failed to update ad",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }
        public async Task NotifyUsersAsync(Ad ad)
        {
            var searches = await _context.SavedSearches
                .Where(s =>
                    (!s.SubcategoryId.HasValue || s.SubcategoryId == ad.SubcategoryId) &&
                    (!s.MinPrice.HasValue || ad.Price >= s.MinPrice) &&
                    (!s.MaxPrice.HasValue || ad.Price <= s.MaxPrice)
                )
                .ToListAsync();

            foreach (var search in searches)
            {
                var user = await _context.Users.FindAsync(search.UserId);
                if (user == null) continue;

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Nowe ogłoszenie spełniające Twoje kryteria",
                    $"Dodano nowe ogłoszenie: {ad.Title}"
                );

                _context.Notifications.Add(new Notification
                {
                    UserId = user.Id,
                    AdId = ad.AdId,
                    Type = "NewAd",               
                    Payload = $"Nowe ogłoszenie: {ad.Title}",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<AdService<bool>> DeleteAdAsync(int id)
        {
            try
            {
                var entity = await _context.Ads.FindAsync(id);
                if (entity == null)
                {
                    return AdService<bool>.BadResult(
                        $"Ad with ID {id} not found",
                        404,
                        data: false);
                }

                _context.Ads.Remove(entity);
                await _context.SaveChangesAsync();

                return AdService<bool>.GoodResult(
                    "Ad deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return AdService<bool>.BadResult(
                    "Failed to delete ad",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<AdService<bool>> AdExistsAsync(int id)
        {
            try
            {
                var exists = await _context.Ads.AnyAsync(a => a.AdId == id);

                return AdService<bool>.GoodResult(
                    exists ? "Ad exists" : "Ad does not exist",
                    200,
                    exists);
            }
            catch (Exception ex)
            {
                return AdService<bool>.BadResult(
                    "Failed to check ad existence",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

    }
    }
