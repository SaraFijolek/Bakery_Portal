using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class SubcategoryReadDto
    {
        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

       
        public CategoryBasicDto? Category { get; set; }
    }

    
    public class SubcategoryCreateDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    
    public class SubcategoryUpdateDto
    {
        [Required]
        public int SubcategoryId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    
    public class SubcategoryBasicDto
    {
        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

   
    public class CategoryBasicDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
       
        
    }
}
