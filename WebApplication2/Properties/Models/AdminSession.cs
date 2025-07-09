using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class AdminSession
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        public int AdminId { get; set; }

        [Required]
        [MaxLength(255)]
        public string SessionToken { get; set; }

        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(500)]
        public string UserAgent { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("AdminId")]
        public virtual Admin Admin { get; set; }
    }
}
