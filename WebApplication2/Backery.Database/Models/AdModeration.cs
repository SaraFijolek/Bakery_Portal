using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;

namespace WebApplication2.Properties.Models
{
    public class AdModeration
    {
        [Key]
        public int ModerationId { get; set; }

        [Required]
        public int AdId { get; set; }

        [Required]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; }

        [Column(TypeName = "TEXT")]
        public string Reason { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("AdId")]
        public virtual Ad Ad { get; set; }

        [ForeignKey("AdminId")]
        public virtual Admin Admin { get; set; }
    }
}
