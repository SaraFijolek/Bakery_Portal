using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public DateTime SentAt { get; set; } = DateTime.Now;

        [Column(TypeName = "TEXT")]
        public string? Content { get; set; }

      
        [ForeignKey("SenderId")]
        public virtual User? Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual User? Receiver { get; set; }
    }
}
