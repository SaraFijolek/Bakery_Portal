using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class AdminRolePermission
    {
        [Key]
        public int RolePermissionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; }

        [Required]
        public int PermissionId { get; set; }

        // Navigation properties
        [ForeignKey("PermissionId")]
        public virtual AdminPermission AdminPermission { get; set; }
    }
}
