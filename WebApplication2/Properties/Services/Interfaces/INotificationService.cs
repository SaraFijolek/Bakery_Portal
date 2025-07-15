using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>>GetNotificationsAsync();
        Task<Notification?>GetNotificationByIdAsync(int id);
        Task<Notification>CreateNotificationAsync(Notification notification);
        Task<Notification>UpdateNotificationAsync(int id, Notification notification);
        Task<bool> DeleteNotificationAsync(int id);
        Task<bool> NotificationExistsAsync(int id);
    }
}
