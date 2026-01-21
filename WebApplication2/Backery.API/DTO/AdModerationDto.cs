using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class AdModerationReadDto
    {
        public int ModerationId { get; set; }
        public int AdId { get; set; }
        public int AdminId { get; set; }
        public string Action { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties data
        public AdDto Ad { get; set; }
        public AdminDto Admin { get; set; }
    }

    public class CreateAdModerationDto
    {
        [Required]
        public int AdId { get; set; }

        [Required]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; }

        public string Reason { get; set; }
    }

    public class UpdateAdModerationDto
    {
        [Required]
        public int AdId { get; set; }

        [Required]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; }

        public string Reason { get; set; }
    }
}
