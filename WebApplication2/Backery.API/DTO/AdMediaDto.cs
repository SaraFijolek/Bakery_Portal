using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    using System.ComponentModel.DataAnnotations;

    
    
        public class AdMediaDto
        {
            public int MediaId { get; set; }
            public int AdId { get; set; }
            public string Url { get; set; } = string.Empty;
            public string? MediaType { get; set; }


            public AdDto? Ad { get; set; }
        }





        public class AdDto
        {
            public int AdId { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Category { get; set; }
            public decimal? Price { get; set; }
            public string? Location { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsActive { get; set; }
           public int Status {  get; set; }
        }





        public class CreateAdMediaDto
        {
            [Required]
            public int AdId { get; set; }

            [Required]
            [StringLength(255)]
            public string Url { get; set; } = string.Empty;

            [StringLength(50)]
            public string? MediaType { get; set; }
        }





        public class UpdateAdMediaDto
        {
            [Required]
            public int MediaId { get; set; }

            [Required]
            public int AdId { get; set; }

            [Required]
            [StringLength(255)]
            public string Url { get; set; } = string.Empty;

            [StringLength(50)]
            public string? MediaType { get; set; }
        }



        public class AdMediaResponseDto
        {
            public int MediaId { get; set; }
            public int AdId { get; set; }
            public string Url { get; set; } = string.Empty;
            public string? MediaType { get; set; }


            public AdBasicDto? Ad { get; set; }
        }




        public class AdBasicDto
        {
            public int AdId { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Category { get; set; }
            public decimal? Price { get; set; }
            public string? Location { get; set; }
            public bool IsActive { get; set; }
        }


    }

