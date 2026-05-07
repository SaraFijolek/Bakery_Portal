using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Properties.DTOs
{
    
    public class RatingDto
    {
        public int RatingId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public byte Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FromUserName { get; set; } = string.Empty;
        public string ToUserName { get; set; } = string.Empty;
    }

    
    public class CreateRatingDto
    {
        [Required(ErrorMessage = "FromUserId is required")]
        public string FromUserId { get; set; }

        [Required(ErrorMessage = "ToUserId is required")]
        public string ToUserId { get; set; }

        [Required(ErrorMessage = "Score is required")]
        [Range(1, 5, ErrorMessage = "Score must be between 1 and 5")]
        public byte Score { get; set; }
    }

    
    public class UpdateRatingDto
    {
        [Required(ErrorMessage = "FromUserId is required")]
        public string FromUserId { get; set; }

        [Required(ErrorMessage = "ToUserId is required")]
        public string ToUserId { get; set; }

        [Required(ErrorMessage = "Score is required")]
        [Range(1, 5, ErrorMessage = "Score must be between 1 and 5")]
        public byte Score { get; set; }
    }

    
    public class RatingListDto
    {
        public int RatingId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public byte Score { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}