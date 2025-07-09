using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;
namespace WebApplication2.Properties.Models
{
    public class Admedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MediaId { get; set; }

        [Required]
        public int AdId { get; set; }

        [Required]
        [StringLength(255)]
        public string Url { get; set; } = string.Empty;

        [StringLength(50)]
        public string? MediaType { get; set; }

       
        [ForeignKey("AdId")]
        public virtual Ad? Ad { get; set; }
    }
}

