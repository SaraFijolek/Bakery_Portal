using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Properties.DTOs
{
    
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string UserId { get; set; }
        public int? AdId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string AdTitle { get; set; } = string.Empty;
    }

   
    public class CreateNotificationDto
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }

        public int? AdId { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;
    }

   
    public class UpdateNotificationDto
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }

        public int? AdId { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        [Required(ErrorMessage = "IsRead is required")]
        public bool IsRead { get; set; }
    }

    
    public class MarkAsReadDto
    {
        [Required(ErrorMessage = "IsRead is required")]
        public bool IsRead { get; set; }
    }

   
    public class NotificationListDto
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}