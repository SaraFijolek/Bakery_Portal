using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class UserModeration
    {
        [Key]
        public int ModerationId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; }

        [Column(TypeName = "TEXT")]
        public string Reason { get; set; }

        public int? DurationHours { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ExpiresAt { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("AdminId")]
        public virtual Admin Admin { get; set; }
    }
}
