using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class CommentReadDto
    {
        public int CommentId { get; set; }
        public int AdId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Content { get; set; }

        // Navigation properties as nested DTOs
        public AdBasicDto? Ad { get; set; }
        public UserBasicDto? User { get; set; }
    }

    
    public class CommentCreateDto
    {
        [Required]
        public int AdId { get; set; }

        [Required]
        public string UserId { get; set; }

        public string? Content { get; set; }

        
    }

    public class CommentUpdateDto
    {
        [Required]
        public int CommentId { get; set; }

        [Required]
        public int AdId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public string? Content { get; set; }
    }

  
    public class CommentBasicDto
    {
        public int CommentId { get; set; }
        public int AdId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Content { get; set; }
    }

    
    public class UserBasicDto
    {
        public string UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
       
    }
}

