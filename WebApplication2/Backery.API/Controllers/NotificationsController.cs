using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;


namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
        {
            var notifications = await _notificationService.GetNotificationsAsync();
            return Ok(notifications);
        }

        // GET: api/Notifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetNotification(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
                return NotFound();
            return Ok(notification);
        }

        // POST: api/Notifications
        [HttpPost]
        public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationDto createNotificationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdNotification = await _notificationService.CreateNotificationAsync(createNotificationDto);
            return CreatedAtAction(nameof(GetNotification), new { id = createdNotification.NotificationId }, createdNotification);
        }

        // PUT: api/Notifications/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, UpdateNotificationDto updateNotificationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedNotification = await _notificationService.UpdateNotificationAsync(id, updateNotificationDto);
                return Ok(updatedNotification);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/Notifications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var deleted = await _notificationService.DeleteNotificationAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
