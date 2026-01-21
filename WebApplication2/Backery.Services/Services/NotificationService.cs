using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Properties.Services
{


    public class NotificationsService<T>
    {
        public bool Success { get; set; }
        public T ? Data { get; set; }
        public string ? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int  StatusCode { get; set; }

        public static NotificationsService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new NotificationsService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };
        public static NotificationsService<T> BadResult(
           string message,
           int statusCode,
           List<string>? errors = null,
           T? data = default)
            => new NotificationsService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
            
    }
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationsService<IEnumerable<NotificationDto>>>GetNotificationsAsync()
        {
            try
            {
                var notifications = await _context.Notifications
                    .Include(n => n.User)
                    .Include(n => n.Ad)
                    .ToListAsync();

                var notificationDtos = notifications.Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    UserId = n.UserId,
                    AdId = n.AdId,
                    Type = n.Type,
                    Payload = n.Payload,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    UserName = n.User?.UserName ?? string.Empty,
                    AdTitle = n.Ad?.Title ?? string.Empty
                });

                return NotificationsService<IEnumerable<NotificationDto>>.GoodResult(
                    "Notifications retrieved successfully",
                    200,
                    notificationDtos);
            }
            catch (Exception ex)
            {
                return NotificationsService<IEnumerable<NotificationDto>>.BadResult(
                    "Failed to retrieve notifications",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<NotificationsService<NotificationDto>> GetNotificationByIdAsync(int id)
        {
            try
            {
                var notification = await _context.Notifications
                    .Include(n => n.User)
                    .Include(n => n.Ad)
                    .FirstOrDefaultAsync(n => n.NotificationId == id);

                if (notification == null)
                {
                    return NotificationsService<NotificationDto>.BadResult(
                        $"Notification with ID {id} not found",
                        404);
                }

                var notificationDto = new NotificationDto
                {
                    NotificationId = notification.NotificationId,
                    UserId = notification.UserId,
                    AdId = notification.AdId,
                    Type = notification.Type,
                    Payload = notification.Payload,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt,
                    UserName = notification.User?.UserName ?? string.Empty,
                    AdTitle = notification.Ad?.Title ?? string.Empty
                };

                return NotificationsService<NotificationDto>.GoodResult(
                    "Notification retrieved successfully",
                    200,
                    notificationDto);
            }
            catch (Exception ex)
            {
                return NotificationsService<NotificationDto>.BadResult(
                    "Failed to retrieve notification",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<NotificationsService<NotificationDto>> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            try
            {
                // Validate that UserId and AdId exist
                var userExists = await _context.Users.AnyAsync(u => u.Id == createNotificationDto.UserId);
                var adExists = createNotificationDto.AdId.HasValue ?
                    await _context.Ads.AnyAsync(a => a.AdId == createNotificationDto.AdId) : true;

                var errors = new List<string>();
                if (!userExists)
                    errors.Add($"User with ID {createNotificationDto.UserId} does not exist");
                if (!adExists)
                    errors.Add($"Ad with ID {createNotificationDto.AdId} does not exist");

                if (errors.Any())
                {
                    return NotificationsService<NotificationDto>.BadResult(
                        "Validation failed",
                        400,
                        errors);
                }

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

                var notificationDto = new NotificationDto
                {
                    NotificationId = createdNotification.NotificationId,
                    UserId = createdNotification.UserId,
                    AdId = createdNotification.AdId,
                    Type = createdNotification.Type,
                    Payload = createdNotification.Payload,
                    IsRead = createdNotification.IsRead,
                    CreatedAt = createdNotification.CreatedAt,
                    UserName = createdNotification.User?.UserName ?? string.Empty,
                    AdTitle = createdNotification.Ad?.Title ?? string.Empty
                };

                return NotificationsService<NotificationDto>.GoodResult(
                    "Notification created successfully",
                    201,
                    notificationDto);
            }
            catch (Exception ex)
            {
                return NotificationsService<NotificationDto>.BadResult(
                    "Failed to create notification",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<NotificationsService<NotificationDto>> UpdateNotificationAsync(int id, UpdateNotificationDto updateNotificationDto)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return NotificationsService<NotificationDto>.BadResult(
                        $"Notification with ID {id} not found",
                        404);
                }

                // Validate that UserId and AdId exist
                var userExists = await _context.Users.AnyAsync(u => u.Id == updateNotificationDto.UserId);
                var adExists = updateNotificationDto.AdId.HasValue ?
                    await _context.Ads.AnyAsync(a => a.AdId == updateNotificationDto.AdId) : true;

                var errors = new List<string>();
                if (!userExists)
                    errors.Add($"User with ID {updateNotificationDto.UserId} does not exist");
                if (!adExists)
                    errors.Add($"Ad with ID {updateNotificationDto.AdId} does not exist");

                if (errors.Any())
                {
                    return NotificationsService<NotificationDto>.BadResult(
                        "Validation failed",
                        400,
                        errors);
                }

                notification.UserId = updateNotificationDto.UserId;
                notification.AdId = updateNotificationDto.AdId;
                notification.Type = updateNotificationDto.Type;
                notification.Payload = updateNotificationDto.Payload;
                notification.IsRead = updateNotificationDto.IsRead;

                _context.Entry(notification).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                // Load the updated notification with navigation properties
                var updatedNotification = await _context.Notifications
                    .Include(n => n.User)
                    .Include(n => n.Ad)
                    .FirstOrDefaultAsync(n => n.NotificationId == id);

                var notificationDto = new NotificationDto
                {
                    NotificationId = updatedNotification.NotificationId,
                    UserId = updatedNotification.UserId,
                    AdId = updatedNotification.AdId,
                    Type = updatedNotification.Type,
                    Payload = updatedNotification.Payload,
                    IsRead = updatedNotification.IsRead,
                    CreatedAt = updatedNotification.CreatedAt,
                    UserName = updatedNotification.User?.UserName ?? string.Empty,
                    AdTitle = updatedNotification.Ad?.Title ?? string.Empty
                };

                return NotificationsService<NotificationDto>.GoodResult(
                    "Notification updated successfully",
                    200,
                    notificationDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await NotificationExistsAsync(id))
                {
                    return NotificationsService<NotificationDto>.BadResult(
                        $"Notification with ID {id} not found",
                        404);
                }

                return NotificationsService<NotificationDto>.BadResult(
                    "Concurrency conflict occurred while updating notification",
                    409);
            }
            catch (Exception ex)
            {
                return NotificationsService<NotificationDto>.BadResult(
                    "Failed to update notification",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<NotificationsService<bool>> DeleteNotificationAsync(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return NotificationsService<bool>.BadResult(
                        $"Notification with ID {id} not found",
                        404,
                        data: false);
                }

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                return NotificationsService<bool>.GoodResult(
                    "Notification deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return NotificationsService<bool>.BadResult(
                    "Failed to delete notification",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<bool> NotificationExistsAsync(int id)
        {
            return await _context.Notifications.AnyAsync(e => e.NotificationId == id);
        }
    }
}
