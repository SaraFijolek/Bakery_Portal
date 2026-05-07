using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class UserSocialAuthDto
    {
        public int SocialAuthId { get; set; }
        public string UserId { get; set; }
        public string Provider { get; set; }
        public string ProviderId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? TokenExpires { get; set; }
        public string ProfileData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserDto User { get; set; }
    }


    public class CreateUserSocialAuthDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Provider { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProviderId { get; set; }

        [MaxLength(500)]
        public string AccessToken { get; set; }

        [MaxLength(500)]
        public string RefreshToken { get; set; }

        public DateTime? TokenExpires { get; set; }

        public string ProfileData { get; set; }
    }

    public class UpdateUserSocialAuthDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Provider { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProviderId { get; set; }

        [MaxLength(500)]
        public string AccessToken { get; set; }

        [MaxLength(500)]
        public string RefreshToken { get; set; }

        public DateTime? TokenExpires { get; set; }

        public string ProfileData { get; set; }
    }

    

}

