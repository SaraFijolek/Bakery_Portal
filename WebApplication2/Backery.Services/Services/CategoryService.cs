using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                Name = createCategoryDto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found");

            category.Name = updateCategoryDto.Name;
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CategoryExistsAsync(id))
                    throw new KeyNotFoundException($"Category with ID {id} not found");
                throw;
            }

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(e => e.CategoryId == id);
        }
    }
}