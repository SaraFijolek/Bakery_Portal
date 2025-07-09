
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApplication2.Properties.Models
{
    public class User 
    {
        
    
        
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int UserId { get; set; }

            [Required]
            [MaxLength(255)]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [MaxLength(255)]
            public string? PasswordHash { get; set; }

            [MaxLength(100)]
            public string? Name { get; set; }

            [MaxLength(50)]
            public string? Phone { get; set; }

            [MaxLength(255)]
            public string? AvatarUrl { get; set; }

            // Logowanie społecznościowe
            [Required]
            [MaxLength(50)]
            public string AuthProvider { get; set; } = "email";

            [MaxLength(255)]
            public string? GoogleId { get; set; }

            [MaxLength(255)]
            public string? FacebookId { get; set; }

            // Weryfikacja konta
            public bool IsEmailVerified { get; set; } = false;

            [MaxLength(255)]
            public string? EmailVerificationToken { get; set; }

            public DateTime? EmailVerificationExpires { get; set; }

            // Resetowanie hasła
            [MaxLength(255)]
            public string? ResetPasswordToken { get; set; }

            public DateTime? ResetPasswordExpires { get; set; }

            // Metadane
            [Required]
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public DateTime? LastLogin { get; set; }

            // Metody pomocnicze
            public bool IsPasswordResetTokenValid()
            {
                return !string.IsNullOrEmpty(ResetPasswordToken) &&
                       ResetPasswordExpires.HasValue &&
                       ResetPasswordExpires.Value > DateTime.UtcNow;
            }

            public bool IsEmailVerificationTokenValid()
            {
                return !string.IsNullOrEmpty(EmailVerificationToken) &&
                       EmailVerificationExpires.HasValue &&
                       EmailVerificationExpires.Value > DateTime.UtcNow;
            }

            public bool IsEmailAuthProvider()
            {
                return AuthProvider == "email";
            }

            public bool IsGoogleAuthProvider()
            {
                return AuthProvider == "google";
            }

            public bool IsFacebookAuthProvider()
            {
                return AuthProvider == "facebook";
            }
        }
    }

