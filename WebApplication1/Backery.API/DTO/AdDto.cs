using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class AdResponseDto
    {
        public int AdId { get; set; }
        public string UserId { get; set; }
        public int SubcategoryId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Status { get; set; }

        
        public UserDto? User { get; set; }
        public SubcategoryDto? Subcategory { get; set; }
    }

    public class AdCreateDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int SubcategoryId { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        [MaxLength(255)]
        public string? Location { get; set; }

        public DateTime? ExpiresAt { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }
    }

    public class AdUpdateDto
    {
        [Required]
        public int AdId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int SubcategoryId { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        [MaxLength(255)]
        public string? Location { get; set; }

        public DateTime? ExpiresAt { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }
    }

  
   
    public class SubcategoryDto
    {
        public int SubcategoryId { get; set; }
        public string Name { get; set; } = null!;

    }
}

