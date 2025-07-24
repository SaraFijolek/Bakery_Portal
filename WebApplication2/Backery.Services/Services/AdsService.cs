using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Models;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdsService : IAdsService
    {
        private readonly AppDbContext _context;

        public AdsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdResponseDto>> GetAllAdsAsync()
        {
            var ads = await _context.Ads
                .Include(a => a.User)
                .Include(a => a.Subcategory)
                .ToListAsync();

            return ads.Select(a => new AdResponseDto
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
                    UserId = a.User.UserId,
                    Username = a.User.Name
                    // map other needed fields
                },
                Subcategory = a.Subcategory == null ? null : new SubcategoryDto
                {
                    SubcategoryId = a.Subcategory.SubcategoryId,
                    Name = a.Subcategory.Name
                    // map other needed fields
                }
            });
        }

        public async Task<AdResponseDto?> GetAdByIdAsync(int id)
        {
            var a = await _context.Ads
                .Include(ad => ad.User)
                .Include(ad => ad.Subcategory)
                .FirstOrDefaultAsync(ad => ad.AdId == id);

            if (a == null) return null;

            return new AdResponseDto
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
                    UserId = a.User.UserId,
                    Username = a.User.Name
                    // map other needed fields
                },
                Subcategory = a.Subcategory == null ? null : new SubcategoryDto
                {
                    SubcategoryId = a.Subcategory.SubcategoryId,
                    Name = a.Subcategory.Name
                    // map other needed fields
                }
            };
        }

        public async Task<AdResponseDto> CreateAdAsync(AdCreateDto dto)
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

            await _context.Entry(entity).Reference(a => a.User).LoadAsync();
            await _context.Entry(entity).Reference(a => a.Subcategory).LoadAsync();

            return new AdResponseDto
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
                    UserId = entity.User.UserId,
                    Username = entity.User.Name
                    // map other needed fields
                },
                Subcategory = entity.Subcategory == null ? null : new SubcategoryDto
                {
                    SubcategoryId = entity.Subcategory.SubcategoryId,
                    Name = entity.Subcategory.Name
                    // map other needed fields
                }
            };
        }

        public async Task<bool> UpdateAdAsync(AdUpdateDto dto)
        {
            var exists = await _context.Ads.AnyAsync(a => a.AdId == dto.AdId);
            if (!exists) return false;

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

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAdAsync(int id)
        {
            var entity = await _context.Ads.FindAsync(id);
            if (entity == null) return false;

            _context.Ads.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdExistsAsync(int id)
        {
            return await _context.Ads.AnyAsync(a => a.AdId == id);
        }
    }
}
