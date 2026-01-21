using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class AdminRolePermissionResponseDto
    {
        public int RolePermissionId { get; set; }
        public string Role { get; set; }
        public int PermissionId { get; set; }

        // Zagnieżdżone dane uprawnień
        public AdminPermissionDto AdminPermission { get; set; }
    }

    // DTO dla tworzenia nowego AdminRolePermission
    public class CreateAdminRolePermissionDto
    {
        [Required]
        [MaxLength(50)]
        public string Role { get; set; }

        [Required]
        public int PermissionId { get; set; }
    }

    // DTO dla aktualizacji AdminRolePermission
    public class UpdateAdminRolePermissionDto
    {
        [Required]
        [MaxLength(50)]
        public string Role { get; set; }

        [Required]
        public int PermissionId { get; set; }
    }

    // DTO dla listy rol i uprawnień (uproszczona wersja)
    public class AdminRolePermissionListItemDto
    {
        public int RolePermissionId { get; set; }
        public string Role { get; set; }
        public int PermissionId { get; set; }

        // Tylko podstawowe dane uprawnień
        public string PermissionName { get; set; }
        public string PermissionCategory { get; set; }
    }

    // DTO dla zagnieżdżonych danych AdminPermission
    public class AdminPermissionDto
    {
        public int PermissionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
