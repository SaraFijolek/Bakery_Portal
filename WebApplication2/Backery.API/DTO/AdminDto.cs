using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class CreateAdminDto
    {
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
        public string? Phone { get; set; }

        [MaxLength(255)]
        public string? AvatarUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "moderator";

        public bool IsActive { get; set; } = true;

        public bool TwoFactorEnabled { get; set; } = false;

        [MaxLength(255)]
        public string? TwoFactorSecret { get; set; }

        public string? BackupCodes { get; set; }
    }

    // DTO for updating an existing admin
    public class UpdateAdminDto
    {
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(255)]
        public string? PasswordHash { get; set; } // Optional for updates

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(255)]
        public string? AvatarUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool TwoFactorEnabled { get; set; }

        [MaxLength(255)]
        public string? TwoFactorSecret { get; set; }

        public string? BackupCodes { get; set; }
    }

    // DTO for returning admin data (with navigation properties)
    public class AdminDto
    {
        public string AdminId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }

        // Navigation properties as DTOs
        public List<AdminSessionDto>? AdminSessions { get; set; }
        public List<AdminAuditLogDto>? AdminAuditLogs { get; set; }
        public List<UserModerationDto>? UserModerations { get; set; }
        public List<AdModerationDto>? AdModerations { get; set; }
        public List<ReportedContentDto>? ReportedContents { get; set; }
        public string Username { get; internal set; }
    }

    // DTO for admin listing (without sensitive data and navigation properties)
    public class AdminListDto
    {
        public string AdminId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }

        // Summary counts instead of full navigation properties
        public int SessionsCount { get; set; }
        public int AuditLogsCount { get; set; }
        public int UserModerationsCount { get; set; }
        public int AdModerationsCount { get; set; }
        public int ReportedContentsCount { get; set; }
    }

    // DTO for admin profile (without sensitive security data)
    public class AdminProfileDto
    {
        public string AdminId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    // Supporting DTOs for navigation properties (simplified versions)
    public class AdminSessionDto
    {
        public int SessionId { get; set; }
        public string SessionToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }

    public class AdminAuditLogDto
    {
        public int AuditId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime CreatedAt { get; set; }
        public string IpAddress { get; set; }
    }

    public class AdModerationDto
    {
        public int ModerationId { get; set; }
        public int AdId { get; set; }
        public string Action { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class ReportedContentDto
    {
        public int ReportId { get; set; }
        public int ReportedBy { get; set; }
        public string ContentType { get; set; }
        public int ContentId { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    // DTO for password change operations
    public class ChangeAdminPasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [MaxLength(255)]
        public string NewPasswordHash { get; set; }
    }

    // DTO for 2FA setup
    public class Setup2FADto
    {
        [Required]
        public bool TwoFactorEnabled { get; set; }

        [MaxLength(255)]
        public string? TwoFactorSecret { get; set; }

        public string? BackupCodes { get; set; }
    }

   
}
