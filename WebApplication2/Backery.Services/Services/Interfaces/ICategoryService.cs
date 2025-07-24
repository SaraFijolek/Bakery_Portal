using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
    }
}
