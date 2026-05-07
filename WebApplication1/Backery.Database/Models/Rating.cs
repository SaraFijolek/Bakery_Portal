using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }

        [Required]
        public string FromUserId { get; set; }

        [Required]
        public string ToUserId { get; set; }

        [Required]
        public byte Score { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("FromUserId")]
        public virtual User FromUser { get; set; }

        [ForeignKey("ToUserId")]
        public virtual User ToUser { get; set; }
    }
}
