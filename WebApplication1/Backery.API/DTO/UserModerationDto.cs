using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{

    public class CreateUserModerationDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; }

        public string? Reason { get; set; }

        public int? DurationHours { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;
    }


    public class UpdateUserModerationDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; }

        public string? Reason { get; set; }

        public int? DurationHours { get; set; }

        public DateTime? ExpiresAt { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }


    public class UserModerationDto
    {
        public int ModerationId { get; set; }
        public string UserId { get; set; }
        public string AdminId { get; set; }
        public string Action { get; set; }
        public string? Reason { get; set; }
        public int? DurationHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }


        public UserDto? User { get; set; }

        public AdminDto? Admin { get; set; }
    }




    public class UserModerationListDto
    {
        public int ModerationId { get; set; }
        public string UserId { get; set; }
        public string AdminId { get; set; }
        public string Action { get; set; }
        public string? Reason { get; set; }
        public int? DurationHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }


        public string UserName { get; set; }
        public string AdminName { get; set; }
    }

    public class UserDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

    }

}




