using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface ISubcategoriesService
    {

        Task<SubcategoryService<IEnumerable<SubcategoryReadDto>>> GetAllSubcategoriesAsync();
        Task<SubcategoryService<SubcategoryReadDto?>> GetSubcategoryAsync(int id);
        Task<SubcategoryService<SubcategoryReadDto>> CreateSubcategoryAsync(SubcategoryCreateDto subcategoryCreateDto);
        Task<SubcategoryService<SubcategoryReadDto>> UpdateSubcategoryAsync(int id, SubcategoryUpdateDto subcategoryUpdateDto);
        Task<SubcategoryService<bool>> DeleteSubcategoryAsync(int id);
        Task<bool> SubcategoryExistsAsync(int id);
    }
}
