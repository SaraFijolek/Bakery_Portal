using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class AdminAuditLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public int AdminId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; }

        [MaxLength(50)]
        public string TargetType { get; set; }

        public int? TargetId { get; set; }

        [Column(TypeName = "TEXT")]
        public string Description { get; set; }

        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(500)]
        public string UserAgent { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("AdminId")]
        public virtual Admin Admin { get; set; }
    }
}
