
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApplication2.Properties.Models
{
    public class User : IdentityUser
    {
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

        public ICollection<Message> SentMessages { get; set; } = new List<Message>();

        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        public ICollection<Rating> GivenRatings { get; set; } = new List<Rating>();

        public ICollection<Rating> ReceivedRatings { get; set; } = new List<Rating>();

        public ICollection<AdminSession> AdminSessions { get; set; }
        
        public ICollection<ReportedContent> ReportedContent { get; set; }

        public ICollection<ReportedContent> ReportedContentAdmins { get; set; }

        public ICollection<AdminAuditLog> AdminAuditLogs { get; set; }

        public ICollection<UserModeration> UserModerations { get; set; }

        public ICollection<AdModeration> AdModerations { get; set; }

        public ICollection<ReportedContent> ReportedContents { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
    }

