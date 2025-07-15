using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
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

        public async Task<IEnumerable<Ad>> GetAllAdsAsync()
        {
            return await _context.Ads
                .Include(a => a.User)
                .Include(a => a.Subcategory)
                .ToListAsync();
        }

        public async Task<Ad?> GetAdByIdAsync(int id)
        {
            return await _context.Ads
                .Include(a => a.User)
                .Include(a => a.Subcategory)
                .FirstOrDefaultAsync(a => a.AdId == id);
        }

        public async Task<Ad> CreateAdAsync(Ad ad)
        {
            _context.Ads.Add(ad);
            await _context.SaveChangesAsync();
            return ad;
        }

        public async Task<bool> UpdateAdAsync(int id, Ad ad)
        {
            if (id != ad.AdId)
            {
                return false;
            }

            _context.Entry(ad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdExistsAsync(id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteAdAsync(int id)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return false;
            }

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdExistsAsync(int id)
        {
            return await _context.Ads.AnyAsync(e => e.AdId == id);
        }
    }
}

