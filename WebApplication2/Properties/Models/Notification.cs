using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;

namespace WebApplication2.Properties.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? AdId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Column(TypeName = "TEXT")]
        public string Payload { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("AdId")]
        public virtual Ad Ad { get; set; }
    }
}
