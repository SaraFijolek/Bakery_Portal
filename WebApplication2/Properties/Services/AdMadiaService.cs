using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class AdMadiaService : IAdMadiaService
    {
        private readonly AppDbContext _context;

        public AdMadiaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Admedia>> GetAllAdMediaAsync()
        {
            return await _context.Admedias
                .Include(m => m.Ad)
                .ToListAsync();
        }

        public async Task<Admedia?> GetAdMediaByIdAsync(int id)
        {
            return await _context.Admedias
                .Include(m => m.Ad)
                .FirstOrDefaultAsync(m => m.MediaId == id);
        }

        public async Task<Admedia> CreateAdMediaAsync(Admedia adMedia)
        {
            _context.Admedias.Add(adMedia);
            await _context.SaveChangesAsync();
            return adMedia;
        }

        public async Task<bool> UpdateAdMediaAsync(int id, Admedia adMedia)
        {
            if (id != adMedia.MediaId)
            {
                return false;
            }

            _context.Entry(adMedia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AdMediaExistsAsync(id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteAdMediaAsync(int id)
        {
            var adMedia = await _context.Admedias.FindAsync(id);
            if (adMedia == null)
            {
                return false;
            }

            _context.Admedias.Remove(adMedia);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdMediaExistsAsync(int id)
        {
            return await _context.Admedias.AnyAsync(e => e.MediaId == id);
        }
    }
}
