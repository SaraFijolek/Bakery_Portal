using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class ReportedContentReadDto
    {
        public int ReportId { get; set; }
        public string? ReporterUserId { get; set; }
        public string ReporterEmail { get; set; }
        public string ContentType { get; set; }
        public int ContentId { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string? AdminId { get; set; }
        public string AdminNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        // Navigation properties data
        public UserDto ReporterUser { get; set; }
        public AdminDto Admin { get; set; }
    }

    public class CreateReportedContentDto
    {
        public string? ReporterUserId { get; set; }

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

        public string Description { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "pending";

        public string? AdminId { get; set; }

        public string AdminNotes { get; set; }

        public DateTime? ResolvedAt { get; set; }
    }

    public class UpdateReportedContentDto
    {
        public string? ReporterUserId { get; set; }

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

        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        public string? AdminId { get; set; }

        public string AdminNotes { get; set; }

        public DateTime? ResolvedAt { get; set; }
    }

    
}
