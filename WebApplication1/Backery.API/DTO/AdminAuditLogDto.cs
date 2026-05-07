using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class AdminAuditLogResponseDto
    {
        public int LogId { get; set; }
        public string AdminId { get; set; }
        public string Action { get; set; }
        public string TargetType { get; set; }
        public int? TargetId { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }

        // Zagnieżdżone dane administratora
        public AdminDto Admin { get; set; }
    }

    // DTO dla tworzenia nowego AdminAuditLog
    public class CreateAdminAuditLogDto
    {
        [Required]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; }

        [MaxLength(50)]
        public string TargetType { get; set; }

        public int? TargetId { get; set; }

        public string Description { get; set; }

        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(500)]
        public string UserAgent { get; set; }
    }

    // DTO dla aktualizacji AdminAuditLog
    public class UpdateAdminAuditLogDto
    {
        [Required]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; }

        [MaxLength(50)]
        public string TargetType { get; set; }

        public int? TargetId { get; set; }

        public string Description { get; set; }

        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(500)]
        public string UserAgent { get; set; }
    }


    // DTO dla listy logów (może być uproszczona wersja bez zagnieżdżonych danych)
    public class AdminAuditLogListItemDto
    {
        public int LogId { get; set; }
        public string AdminId { get; set; }
        public string Action { get; set; }
        public string TargetType { get; set; }
        public int? TargetId { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }

        // Tylko podstawowe dane administratora
        public string AdminUsername { get; set; }
        public string AdminEmail { get; set; }
    }
}

