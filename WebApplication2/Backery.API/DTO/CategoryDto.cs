using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Properties.DTOs
{
    
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
    }

    
    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

  
    public class CategoryListDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
