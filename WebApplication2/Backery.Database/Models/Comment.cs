
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;

namespace WebApplication2.Properties.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }

        [Required]
        public int AdId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? Content { get; set; }

       
        [ForeignKey("AdId")]
        public virtual Ad? Ad { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}

