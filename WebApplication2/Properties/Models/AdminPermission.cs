using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class AdminPermission
    {
        [Key]
        public int PermissionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "TEXT")]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }
    }
}
