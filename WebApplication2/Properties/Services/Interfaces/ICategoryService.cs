using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>>GetAllCategoriesAsync();
        Task<Category?>GetCategoryAsync(int id);
        Task<Category>CreateCategoryAsync(Category category);
        Task<Category>UpdateCategoryAsync(int id ,Category category);
        Task<bool>DeleteCategoryAsync(int id);
        Task<bool>CategoryExistsAsync(int id);
    }
}
