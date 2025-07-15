using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class ReportedContent
    {
        [Key]
        public int ReportId { get; set; }

        public int? ReporterUserId { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        public string ReporterEmail { get; set; }

        [Required]
        [MaxLength(50)]
        public string ContentType { get; set; }

        [Required]
        public int ContentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Reason { get; set; }

        [Column(TypeName = "TEXT")]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "pending";

        public int? AdminId { get; set; }

        [Column(TypeName = "TEXT")]
        public string AdminNotes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ResolvedAt { get; set; }

        // Navigation properties
        [ForeignKey("ReporterUserId")]
        public virtual User ReporterUser { get; set; }

        [ForeignKey("AdminId")]
        public virtual Admin Admin { get; set; }
    }
}
