using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class UserSocialAuthService : IUserSocialAuthService
    {
        private readonly AppDbContext _context;

        public UserSocialAuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserSocialAuthDto>> GetUserSocialAuthsAsync()
        {
            var userSocialAuths = await _context.UserSocialAuths
                .Include(ua => ua.User)
                .ToListAsync();

            return userSocialAuths.Select(ua => new UserSocialAuthDto
            {
                SocialAuthId = ua.SocialAuthId,
                UserId = ua.UserId,
                Provider = ua.Provider,
                ProviderId = ua.ProviderId,
                AccessToken = ua.AccessToken,
                RefreshToken = ua.RefreshToken,
                TokenExpires = ua.TokenExpires,
                ProfileData = ua.ProfileData,
                CreatedAt = ua.CreatedAt,
                UpdatedAt = ua.UpdatedAt,
               
                
            }).ToList();
        }

        public async Task<UserSocialAuthDto> GetUserSocialAuthByIdAsync(int id)
        {
            var userSocialAuth = await _context.UserSocialAuths
                .Include(ua => ua.User)
                .FirstOrDefaultAsync(ua => ua.SocialAuthId == id);

            if (userSocialAuth == null)
                return null;

            return new UserSocialAuthDto
            {
                SocialAuthId = userSocialAuth.SocialAuthId,
                UserId = userSocialAuth.UserId,
                Provider = userSocialAuth.Provider,
                ProviderId = userSocialAuth.ProviderId,
                AccessToken = userSocialAuth.AccessToken,
                RefreshToken = userSocialAuth.RefreshToken,
                TokenExpires = userSocialAuth.TokenExpires,
                ProfileData = userSocialAuth.ProfileData,
                CreatedAt = userSocialAuth.CreatedAt,
                UpdatedAt = userSocialAuth.UpdatedAt,
               
            };
        }

        public async Task<UserSocialAuthDto> CreateUserSocialAuthAsync(CreateUserSocialAuthDto createDto)
        {
            var userSocialAuth = new UserSocialAuth
            {
                UserId = createDto.UserId,
                Provider = createDto.Provider,
                ProviderId = createDto.ProviderId,
                AccessToken = createDto.AccessToken,
                RefreshToken = createDto.RefreshToken,
                TokenExpires = createDto.TokenExpires,
                ProfileData = createDto.ProfileData,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.UserSocialAuths.Add(userSocialAuth);
            await _context.SaveChangesAsync();

            // Reload with User data
            var createdAuth = await _context.UserSocialAuths
                .Include(ua => ua.User)
                .FirstOrDefaultAsync(ua => ua.SocialAuthId == userSocialAuth.SocialAuthId);

            return new UserSocialAuthDto
            {
                SocialAuthId = createdAuth.SocialAuthId,
                UserId = createdAuth.UserId,
                Provider = createdAuth.Provider,
                ProviderId = createdAuth.ProviderId,
                AccessToken = createdAuth.AccessToken,
                RefreshToken = createdAuth.RefreshToken,
                TokenExpires = createdAuth.TokenExpires,
                ProfileData = createdAuth.ProfileData,
                CreatedAt = createdAuth.CreatedAt,
                UpdatedAt = createdAuth.UpdatedAt,
                
            };
        }

        public async Task<bool> UpdateUserSocialAuthAsync(int id, UpdateUserSocialAuthDto updateDto)
        {
            var existingAuth = await _context.UserSocialAuths.FindAsync(id);
            if (existingAuth == null)
                return false;

            existingAuth.UserId = updateDto.UserId;
            existingAuth.Provider = updateDto.Provider;
            existingAuth.ProviderId = updateDto.ProviderId;
            existingAuth.AccessToken = updateDto.AccessToken;
            existingAuth.RefreshToken = updateDto.RefreshToken;
            existingAuth.TokenExpires = updateDto.TokenExpires;
            existingAuth.ProfileData = updateDto.ProfileData;
            existingAuth.UpdatedAt = DateTime.Now;

            _context.Entry(existingAuth).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserSocialAuthExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteUserSocialAuthAsync(int id)
        {
            var auth = await _context.UserSocialAuths.FindAsync(id);
            if (auth == null)
                return false;

            _context.UserSocialAuths.Remove(auth);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserSocialAuthExistsAsync(int id)
        {
            return await _context.UserSocialAuths.AnyAsync(e => e.SocialAuthId == id);
        }
    }
}