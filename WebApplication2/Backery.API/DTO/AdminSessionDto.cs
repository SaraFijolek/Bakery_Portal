using System.ComponentModel.DataAnnotations;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.DTOs
{
    // DTO for reading AdminSession data
    public class AdminSessionReadDto
    {
        public int SessionId { get; set; }
        public string AdminId { get; set; }
        public string SessionToken { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation property data
        public AdminDto Admin { get; set; }
    }

    // DTO for creating new AdminSession
    public class CreateAdminSessionDto
    {
        [Required]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(255)]
        public string SessionToken { get; set; }

        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(500)]
        public string UserAgent { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // DTO for updating AdminSession
    public class UpdateAdminSessionDto
    {
        [Required]
        public string AdminId { get; set; }

        [Required]
        [MaxLength(255)]
        public string SessionToken { get; set; }

        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(500)]
        public string UserAgent { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }

    // Basic Admin DTO for navigation property
    public class AdminDto
    {
        public int AdminId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    // Mapper class for converting between models and DTOs
    public static class AdminSessionMapper
    {
        public static AdminSessionReadDto ToDto(AdminSession session)
        {
            return new AdminSessionReadDto
            {
                SessionId = session.SessionId,
                AdminId = session.AdminId,
                SessionToken = session.SessionToken,
                IpAddress = session.IpAddress,
                UserAgent = session.UserAgent,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt,
                IsActive = session.IsActive,
               
            };
        }

        public static AdminSessionListDto ToListDto(AdminSession session)
        {
            return new AdminSessionListDto
            {
                SessionId = session.SessionId,
                AdminId = session.AdminId,
                SessionToken = session.SessionToken,
                IpAddress = session.IpAddress,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt,
                IsActive = session.IsActive,
                AdminUsername = session.Admin?.Name,
                AdminEmail = session.Admin?.Email
            };
        }

        public static AdminSession ToModel(CreateAdminSessionDto dto)
        {
            return new AdminSession
            {
                AdminId = dto.AdminId,
                SessionToken = dto.SessionToken,
                IpAddress = dto.IpAddress,
                UserAgent = dto.UserAgent,
                CreatedAt = DateTime.Now,
                ExpiresAt = dto.ExpiresAt,
                IsActive = dto.IsActive
            };
        }

        public static void UpdateModel(AdminSession session, UpdateAdminSessionDto dto)
        {
            session.AdminId = dto.AdminId;
            session.SessionToken = dto.SessionToken;
            session.IpAddress = dto.IpAddress;
            session.UserAgent = dto.UserAgent;
            session.ExpiresAt = dto.ExpiresAt;
            session.IsActive = dto.IsActive;
        }

        public static List<AdminSessionReadDto> ToDtoList(List<AdminSession> sessions)
        {
            return sessions.Select(ToDto).ToList();
        }

        public static List<AdminSessionListDto> ToListDtoList(List<AdminSession> sessions)
        {
            return sessions.Select(ToListDto).ToList();
        }
    }

   
    public class AdminSessionListDto
    {
        public int SessionId { get; set; }
        public string AdminId { get; set; }
        public string SessionToken { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }

        
        public string AdminUsername { get; set; }
        public string AdminEmail { get; set; }
    }
}