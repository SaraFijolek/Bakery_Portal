using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Properties.Models;

namespace WebApplication2.Models
{
    public class Ad
    {
        [Key]
        public int AdId { get; set; }

        // Relacja z użytkownikiem (wystawiającym ogłoszenie)
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        // Relacja z podkategorią
        [ForeignKey("Subcategory")]
        public int SubcategoryId { get; set; }
        public Subcategory Subcategory { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [MaxLength(255)]
        public string? Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }
    }
}