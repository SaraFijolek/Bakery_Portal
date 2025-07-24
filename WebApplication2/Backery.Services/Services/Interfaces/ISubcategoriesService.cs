using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface ISubcategoriesService
    {

        Task<IEnumerable<SubcategoryReadDto>> GetAllSubcategoriesAsync();
        Task<SubcategoryReadDto?> GetSubcategoryAsync(int id);
        Task<SubcategoryReadDto> CreateSubcategoryAsync(SubcategoryCreateDto subcategoryCreateDto);
        Task<SubcategoryReadDto> UpdateSubcategoryAsync(int id, SubcategoryUpdateDto subcategoryUpdateDto);
        Task<bool> DeleteSubcategoryAsync(int id);
        Task<bool> SubcategoryExistsAsync(int id);
    }
}
