using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetNotificationsAsync();
        Task<NotificationDto?> GetNotificationByIdAsync(int id);
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto);
        Task<NotificationDto> UpdateNotificationAsync(int id, UpdateNotificationDto updateNotificationDto);
        Task<bool> DeleteNotificationAsync(int id);
        Task<bool> NotificationExistsAsync(int id);
    }
}
