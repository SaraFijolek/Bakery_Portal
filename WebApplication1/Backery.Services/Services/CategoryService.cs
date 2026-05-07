using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class ResultService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ResultService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new ResultService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static ResultService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default)
            =>
            new ResultService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResultService<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name
                    })
                    .ToListAsync();

                return ResultService<IEnumerable<CategoryDto>>.GoodResult(
                    "Categories retrieved successfully",
                    200,
                    categories);
            }
            catch (Exception ex)
            {
                return ResultService<IEnumerable<CategoryDto>>.BadResult(
                    "Failed to retrieve categories",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<CategoryDto>> GetCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return ResultService<CategoryDto>.BadResult(
                        $"Category with ID {id} not found",
                        404);
                }

                var categoryDto = new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name
                };

                return ResultService<CategoryDto>.GoodResult(
                    "Category retrieved successfully",
                    200,
                    categoryDto);
            }
            catch (Exception ex)
            {
                return ResultService<CategoryDto>.BadResult(
                    "Failed to retrieve category",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            try
            {
                var category = new Category
                {
                    Name = createCategoryDto.Name
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                var categoryDto = new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name
                };

                return ResultService<CategoryDto>.GoodResult(
                    "Category created successfully",
                    201,
                    categoryDto);
            }
            catch (Exception ex)
            {
                return ResultService<CategoryDto>.BadResult(
                    "Failed to create category",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return ResultService<CategoryDto>.BadResult(
                        $"Category with ID {id} not found",
                        404);
                }

                category.Name = updateCategoryDto.Name;
                _context.Entry(category).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CategoryExistsAsync(id))
                    {
                        return ResultService<CategoryDto>.BadResult(
                            $"Category with ID {id} not found",
                            404);
                    }
                    throw;
                }

                var categoryDto = new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name
                };

                return ResultService<CategoryDto>.GoodResult(
                    "Category updated successfully",
                    200,
                    categoryDto);
            }
            catch (Exception ex)
            {
                return ResultService<CategoryDto>.BadResult(
                    "Failed to update category",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResultService<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return ResultService<bool>.BadResult(
                        $"Category with ID {id} not found",
                        404,
                        null,
                        false);
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return ResultService<bool>.GoodResult(
                    "Category deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return ResultService<bool>.BadResult(
                    "Failed to delete category",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(e => e.CategoryId == id);
        }
    }
}