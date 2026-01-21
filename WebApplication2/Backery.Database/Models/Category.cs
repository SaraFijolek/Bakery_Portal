using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Properties.Models
{
    public class Category
    {
       
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int CategoryId { get; set; }

            [Required]
            [MaxLength(100)]
            public string Name { get; set; } = string.Empty;
        }
    }

