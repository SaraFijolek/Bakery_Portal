using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class Admin
    {
        [Key]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [MaxLength(255)]
        public string AvatarUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "moderator";

        [Required]
        public bool IsActive { get; set; } = true;

        // Google 2FA
        [Required]
        public bool TwoFactorEnabled { get; set; } = false;

        [MaxLength(255)]
        public string TwoFactorSecret { get; set; }

        [Column(TypeName = "TEXT")]
        public string BackupCodes { get; set; }

        // Metadane
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastLogin { get; set; }

        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LockedUntil { get; set; }

        // Navigation properties
        public virtual ICollection<AdminSession> AdminSessions { get; set; }
        public virtual ICollection<AdminAuditLog> AdminAuditLogs { get; set; }
        public virtual ICollection<UserModeration> UserModerations { get; set; }
        public virtual ICollection<AdModeration> AdModerations { get; set; }
        public virtual ICollection<ReportedContent> ReportedContents { get; set; }
    }
}
