using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class SubcategoriesService : ISubcategoriesService
    {
        private readonly AppDbContext _context;

        public SubcategoriesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subcategory>> GetAllSubcategoriesAsync()
        {
            return await _context.Subcategories
                .Include(sc => sc.Category)
                .ToListAsync();
        }

        public async Task<Subcategory?> GetSubcategoryAsync(int id)
        {
            return await _context.Subcategories
                .Include(sc => sc.Category)
                .FirstOrDefaultAsync(sc => sc.SubcategoryId == id);
        }

        public async Task<Subcategory> CreateSubcategoryAsync(Subcategory subcategory)
        {
            _context.Subcategories.Add(subcategory);
            await _context.SaveChangesAsync();
            return subcategory;
        }

        public async Task<Subcategory> UpdateSubcategoryAsync(int id, Subcategory subcategory)
        {
            if (id != subcategory.SubcategoryId)
                throw new ArgumentException("Subcategory ID mismatch");

            _context.Entry(subcategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SubcategoryExistsAsync(id))
                    throw new KeyNotFoundException($"Subcategory with ID {id} not found");
                throw;
            }

            return subcategory;
        }

        public async Task<bool> DeleteSubcategoryAsync(int id)
        {
            var subcategory = await _context.Subcategories.FindAsync(id);
            if (subcategory == null)
                return false;

            _context.Subcategories.Remove(subcategory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubcategoryExistsAsync(int id)
        {
            return await _context.Subcategories.AnyAsync(e => e.SubcategoryId == id);
        }
    }
}
