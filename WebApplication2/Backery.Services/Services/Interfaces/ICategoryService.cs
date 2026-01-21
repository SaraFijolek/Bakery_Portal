using System.Threading.Tasks;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ResultService<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
        Task<ResultService<CategoryDto>> GetCategoryAsync(int id);
        Task<ResultService<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<ResultService<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task<ResultService<bool>> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
    }
}
