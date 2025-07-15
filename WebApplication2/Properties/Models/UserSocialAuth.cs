using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class UserSocialAuth
    {
        [Key]
        public int SocialAuthId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Provider { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProviderId { get; set; }

        [MaxLength(500)]
        public string AccessToken { get; set; }

        [MaxLength(500)]
        public string RefreshToken { get; set; }

        public DateTime? TokenExpires { get; set; }

        [Column(TypeName = "TEXT")]
        public string ProfileData { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
