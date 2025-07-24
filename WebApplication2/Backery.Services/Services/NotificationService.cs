using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync()
        {
            var notifications = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Ad)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                AdId = n.AdId,
                Type = n.Type,
                Payload = n.Payload,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                UserName = n.User?.Name ?? string.Empty,
                AdTitle = n.Ad?.Title ?? string.Empty
            });
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Ad)
                .FirstOrDefaultAsync(n => n.NotificationId == id);

            if (notification == null)
                return null;

            return new NotificationDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                AdId = notification.AdId,
                Type = notification.Type,
                Payload = notification.Payload,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                UserName = notification.User?.Name ?? string.Empty,
                AdTitle = notification.Ad?.Title ?? string.Empty
            };
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            var notification = new Notification
            {
                UserId = createNotificationDto.UserId,
                AdId = createNotificationDto.AdId,
                Type = createNotificationDto.Type,
                Payload = createNotificationDto.Payload,
                IsRead = createNotificationDto.IsRead,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Load the created notification with navigation properties
            var createdNotification = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Ad)
                .FirstOrDefaultAsync(n => n.NotificationId == notification.NotificationId);

            return new NotificationDto
            {
                NotificationId = createdNotification.NotificationId,
                UserId = createdNotification.UserId,
                AdId = createdNotification.AdId,
                Type = createdNotification.Type,
                Payload = createdNotification.Payload,
                IsRead = createdNotification.IsRead,
                CreatedAt = createdNotification.CreatedAt,
                UserName = createdNotification.User?.Name ?? string.Empty,
                AdTitle = createdNotification.Ad?.Title ?? string.Empty
            };
        }

        public async Task<NotificationDto> UpdateNotificationAsync(int id, UpdateNotificationDto updateNotificationDto)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                throw new KeyNotFoundException($"Notification with ID {id} not found");

            notification.UserId = updateNotificationDto.UserId;
            notification.AdId = updateNotificationDto.AdId;
            notification.Type = updateNotificationDto.Type;
            notification.Payload = updateNotificationDto.Payload;
            notification.IsRead = updateNotificationDto.IsRead;

            _context.Entry(notification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await NotificationExistsAsync(id))
                    throw new KeyNotFoundException($"Notification with ID {id} not found");
                throw;
            }

            // Load the updated notification with navigation properties
            var updatedNotification = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Ad)
                .FirstOrDefaultAsync(n => n.NotificationId == id);

            return new NotificationDto
            {
                NotificationId = updatedNotification.NotificationId,
                UserId = updatedNotification.UserId,
                AdId = updatedNotification.AdId,
                Type = updatedNotification.Type,
                Payload = updatedNotification.Payload,
                IsRead = updatedNotification.IsRead,
                CreatedAt = updatedNotification.CreatedAt,
                UserName = updatedNotification.User?.Name ?? string.Empty,
                AdTitle = updatedNotification.Ad?.Title ?? string.Empty
            };
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> NotificationExistsAsync(int id)
        {
            return await _context.Notifications.AnyAsync(e => e.NotificationId == id);
        }
    }
}
