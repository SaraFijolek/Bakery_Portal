using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
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

        public async Task<List<UserSocialAuth>> GetUserSocialAuthsAsync()
        {
            return await _context.UserSocialAuths
                .Include(ua => ua.User)
                .ToListAsync();
        }

        public async Task<UserSocialAuth?> GetUserSocialAuthByIdAsync(int id)
        {
            return await _context.UserSocialAuths
                .Include(ua => ua.User)
                .FirstOrDefaultAsync(ua => ua.SocialAuthId == id);
        }

        public async Task<UserSocialAuth> CreateUserSocialAuthAsync(UserSocialAuth auth)
        {
            _context.UserSocialAuths.Add(auth);
            await _context.SaveChangesAsync();
            return auth;
        }

        public async Task<bool> UpdateUserSocialAuthAsync(int id, UserSocialAuth auth)
        {
            if (id != auth.SocialAuthId)
                return false;

            _context.Entry(auth).State = EntityState.Modified;

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

