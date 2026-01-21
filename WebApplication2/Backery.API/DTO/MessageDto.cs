using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class MessageReadDto
    {
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime SentAt { get; set; }
        public string? Content { get; set; }

        // Navigation properties as nested DTOs
        public UserBasicDto? Sender { get; set; }
        public UserBasicDto? Receiver { get; set; }
    }

    // DTO for creating a new message
    public class MessageCreateDto
    {
        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public string? Content { get; set; }

        // SentAt will be set automatically in the service
    }

    // DTO for updating an existing message
    public class MessageUpdateDto
    {
        [Required]
        public int MessageId { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public DateTime SentAt { get; set; }

        public string? Content { get; set; }
    }

  
    public class MessageBasicDto
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public DateTime SentAt { get; set; }
        public string? Content { get; set; }

    }
    
}
