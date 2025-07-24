using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
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

        public async Task<IEnumerable<SubcategoryReadDto>> GetAllSubcategoriesAsync()
        {
            var subcategories = await _context.Subcategories
                .Include(sc => sc.Category)
                .ToListAsync();

            return subcategories.Select(sc => new SubcategoryReadDto
            {
                SubcategoryId = sc.SubcategoryId,
                CategoryId = sc.CategoryId,
                Name = sc.Name,
                Category = sc.Category != null ? new CategoryBasicDto
                {
                    CategoryId = sc.Category.CategoryId,
                    Name = sc.Category.Name,
                 
                    // Add other Category properties as needed
                } : null
            });
        }

        public async Task<SubcategoryReadDto?> GetSubcategoryAsync(int id)
        {
            var subcategory = await _context.Subcategories
                .Include(sc => sc.Category)
                .FirstOrDefaultAsync(sc => sc.SubcategoryId == id);

            if (subcategory == null)
                return null;

            return new SubcategoryReadDto
            {
                SubcategoryId = subcategory.SubcategoryId,
                CategoryId = subcategory.CategoryId,
                Name = subcategory.Name,
                Category = subcategory.Category != null ? new CategoryBasicDto
                {
                    CategoryId = subcategory.Category.CategoryId,
                    Name = subcategory.Category.Name,
                } : null
            };
        }

        public async Task<SubcategoryReadDto> CreateSubcategoryAsync(SubcategoryCreateDto subcategoryCreateDto)
        {
            var subcategory = new Subcategory
            {
                CategoryId = subcategoryCreateDto.CategoryId,
                Name = subcategoryCreateDto.Name
            };

            _context.Subcategories.Add(subcategory);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            var createdSubcategory = await _context.Subcategories
                .Include(sc => sc.Category)
                .FirstOrDefaultAsync(sc => sc.SubcategoryId == subcategory.SubcategoryId);

            return new SubcategoryReadDto
            {
                SubcategoryId = createdSubcategory!.SubcategoryId,
                CategoryId = createdSubcategory.CategoryId,
                Name = createdSubcategory.Name,
                Category = createdSubcategory.Category != null ? new CategoryBasicDto
                {
                    CategoryId = createdSubcategory.Category.CategoryId,
                    Name = createdSubcategory.Category.Name,
                    
                } : null
            };
        }

        public async Task<SubcategoryReadDto> UpdateSubcategoryAsync(int id, SubcategoryUpdateDto subcategoryUpdateDto)
        {
            if (id != subcategoryUpdateDto.SubcategoryId)
                throw new ArgumentException("Subcategory ID mismatch");

            var existingSubcategory = await _context.Subcategories.FindAsync(id);
            if (existingSubcategory == null)
                throw new KeyNotFoundException($"Subcategory with ID {id} not found");

            // Update properties
            existingSubcategory.CategoryId = subcategoryUpdateDto.CategoryId;
            existingSubcategory.Name = subcategoryUpdateDto.Name;

            _context.Entry(existingSubcategory).State = EntityState.Modified;

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

            // Reload with navigation properties
            var updatedSubcategory = await _context.Subcategories
                .Include(sc => sc.Category)
                .FirstOrDefaultAsync(sc => sc.SubcategoryId == id);

            return new SubcategoryReadDto
            {
                SubcategoryId = updatedSubcategory!.SubcategoryId,
                CategoryId = updatedSubcategory.CategoryId,
                Name = updatedSubcategory.Name,
                Category = updatedSubcategory.Category != null ? new CategoryBasicDto
                {
                    CategoryId = updatedSubcategory.Category.CategoryId,
                    Name = updatedSubcategory.Category.Name,
                   
                } : null
            };
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
