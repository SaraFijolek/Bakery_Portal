using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    // DTO dla zwracania danych AdminPermission
    public class AdminPermissionResponseDto
    {
        public int PermissionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }

    // DTO dla tworzenia nowego AdminPermission
    public class CreateAdminPermissionDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }
    }

    // DTO dla aktualizacji AdminPermission
    public class UpdateAdminPermissionDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }
    }

    // DTO dla listy uprawnień (może być uproszczona wersja)
    public class AdminPermissionListItemDto
    {
        public int PermissionId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
    }
}
