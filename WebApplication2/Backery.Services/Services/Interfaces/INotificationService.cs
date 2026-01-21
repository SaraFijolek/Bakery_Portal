using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationsService<IEnumerable<NotificationDto>>> GetNotificationsAsync();
        Task<NotificationsService<NotificationDto>> GetNotificationByIdAsync(int id);
        Task<NotificationsService<NotificationDto>> CreateNotificationAsync(CreateNotificationDto createNotificationDto);
        Task<NotificationsService<NotificationDto>> UpdateNotificationAsync(int id, UpdateNotificationDto updateNotificationDto);
        Task<NotificationsService<bool>> DeleteNotificationAsync(int id);
        Task<bool> NotificationExistsAsync(int id);
        Task<NotificationsService<IEnumerable<NotificationDto>>> GetForUserAsync(string userId);
    }
}
