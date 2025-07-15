using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface ISubcategoriesService
    {
        Task<IEnumerable<Subcategory>> GetAllSubcategoriesAsync();
        Task<Subcategory?> GetSubcategoryAsync(int id);
        Task<Subcategory> CreateSubcategoryAsync(Subcategory subcategory);
        Task<Subcategory> UpdateSubcategoryAsync(int id, Subcategory subcategory);
        Task<bool> DeleteSubcategoryAsync(int id);
        Task<bool> SubcategoryExistsAsync(int id);
    }
}
