using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        [HttpGet("me")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetMyNotifications()
        {
            // Get the currently logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Call your service method to get notifications for this user
            var result = await _notificationService.GetForUserAsync(userId);

            if (result.Success)
                return Ok(result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }


        // GET: api/Notifications
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
        {
            var result = await _notificationService.GetNotificationsAsync();
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }





        // GET: api/Notifications/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<NotificationDto>> GetNotification(int id)
        {
            var result = await _notificationService.GetNotificationByIdAsync(id);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // POST: api/Notifications
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationDto createNotificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Model validation failed",
                    errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var result = await _notificationService.CreateNotificationAsync(createNotificationDto);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/Notifications/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UpdateNotification(int id, UpdateNotificationDto updateNotificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Model validation failed",
                    errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var result = await _notificationService.UpdateNotificationAsync(id, updateNotificationDto);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // DELETE: api/Notifications/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }
    }
}