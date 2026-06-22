using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class NotificationsControllerTests
    {
        private readonly Mock<INotificationService> _mockService;
        private readonly NotificationsController _controller;

        private static readonly NotificationDto SampleDto = new()
        {
            NotificationId = 1,
            UserId = "user-123",
            AdId = 10,
            Type = "Info",
            Payload = "Test payload",
            IsRead = false,
            CreatedAt = new DateTime(2024, 1, 1),
            UserName = "JohnDoe",
            AdTitle = "Sample Ad"
        };

        private static readonly CreateNotificationDto ValidCreateDto = new()
        {
            UserId = "user-123",
            AdId = 10,
            Type = "Info",
            Payload = "Test payload",
            IsRead = false
        };

        private static readonly UpdateNotificationDto ValidUpdateDto = new()
        {
            UserId = "user-123",
            AdId = 10,
            Type = "Info",
            Payload = "Updated payload",
            IsRead = true
        };

        public NotificationsControllerTests()
        {
            _mockService = new Mock<INotificationService>();
            _controller = new NotificationsController(_mockService.Object);
        }

        private void SetUser(string userId, string role = "User")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        private void InvalidateModel(string field, string error)
            => _controller.ModelState.AddModelError(field, error);

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/Notifications/me  →  zwraca IActionResult, rzutujemy wprost
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetMyNotifications_ReturnsOk_WhenSuccess()
        {
            SetUser("user-123");
            var notifications = new List<NotificationDto> { SampleDto };
            _mockService
                .Setup(s => s.GetForUserAsync("user-123"))
                .ReturnsAsync(NotificationsService<IEnumerable<NotificationDto>>
                    .GoodResult("OK", 200, notifications));

            var result = await _controller.GetMyNotifications() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
            Assert.Equal(notifications, result.Value);
        }

        [Fact]
        public async Task GetMyNotifications_ReturnsError_WhenServiceFails()
        {
            SetUser("user-123");
            _mockService
                .Setup(s => s.GetForUserAsync("user-123"))
                .ReturnsAsync(NotificationsService<IEnumerable<NotificationDto>>
                    .BadResult("Not found", 404));

            var result = await _controller.GetMyNotifications() as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/Notifications  →  ActionResult<IEnumerable<T>>, używamy .Result
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetNotifications_Returns200_WithData_WhenSuccess()
        {
            var list = new List<NotificationDto> { SampleDto };
            _mockService
                .Setup(s => s.GetNotificationsAsync())
                .ReturnsAsync(NotificationsService<IEnumerable<NotificationDto>>
                    .GoodResult("OK", 200, list));

            // ✅ .Result wyłuskuje IActionResult z ActionResult<T>
            var actionResult = await _controller.GetNotifications();
            var result = actionResult.Result as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
            Assert.Equal(list, result.Value);
        }

        [Fact]
        public async Task GetNotifications_Returns500_WhenServiceFails()
        {
            _mockService
                .Setup(s => s.GetNotificationsAsync())
                .ReturnsAsync(NotificationsService<IEnumerable<NotificationDto>>
                    .BadResult("Internal error", 500, new List<string> { "DB error" }));

            var actionResult = await _controller.GetNotifications();
            var result = actionResult.Result as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/Notifications/{id}  →  ActionResult<NotificationDto>, używamy .Result
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetNotification_Returns200_WhenFound()
        {
            _mockService
                .Setup(s => s.GetNotificationByIdAsync(1))
                .ReturnsAsync(NotificationsService<NotificationDto>
                    .GoodResult("OK", 200, SampleDto));

            // ✅ .Result wyłuskuje IActionResult z ActionResult<T>
            var actionResult = await _controller.GetNotification(1);
            var result = actionResult.Result as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
            Assert.Equal(SampleDto, result.Value);
        }

        [Fact]
        public async Task GetNotification_Returns404_WhenNotFound()
        {
            _mockService
                .Setup(s => s.GetNotificationByIdAsync(99))
                .ReturnsAsync(NotificationsService<NotificationDto>
                    .BadResult("Notification with ID 99 not found", 404));

            var actionResult = await _controller.GetNotification(99);
            var result = actionResult.Result as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/Notifications  →  ActionResult<NotificationDto>, używamy .Result
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateNotification_Returns201_WhenValid()
        {
            _mockService
                .Setup(s => s.CreateNotificationAsync(ValidCreateDto))
                .ReturnsAsync(NotificationsService<NotificationDto>
                    .GoodResult("Created", 201, SampleDto));

            var actionResult = await _controller.CreateNotification(ValidCreateDto);
            var result = actionResult.Result as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(201, result!.StatusCode);
            Assert.Equal(SampleDto, result.Value);
        }

        [Fact]
        public async Task CreateNotification_Returns400_WhenModelInvalid()
        {
            InvalidateModel("UserId", "UserId is required");

            var actionResult = await _controller.CreateNotification(new CreateNotificationDto());
            var result = actionResult.Result as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            _mockService.Verify(s => s.CreateNotificationAsync(It.IsAny<CreateNotificationDto>()), Times.Never);
        }

        [Fact]
        public async Task CreateNotification_Returns400_WhenValidationFails()
        {
            _mockService
                .Setup(s => s.CreateNotificationAsync(ValidCreateDto))
                .ReturnsAsync(NotificationsService<NotificationDto>
                    .BadResult("Validation failed", 400, new List<string> { "User does not exist" }));

            var actionResult = await _controller.CreateNotification(ValidCreateDto);
            var result = actionResult.Result as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // PUT /api/Notifications/{id}  →  IActionResult, rzutujemy wprost
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateNotification_Returns200_WhenSuccess()
        {
            
            var updatedDto = new NotificationDto
            {
                NotificationId = SampleDto.NotificationId,
                UserId = SampleDto.UserId,
                AdId = SampleDto.AdId,
                Type = SampleDto.Type,
                Payload = SampleDto.Payload,
                IsRead = true,   // zmieniona wartość
                CreatedAt = SampleDto.CreatedAt,
                UserName = SampleDto.UserName,
                AdTitle = SampleDto.AdTitle
            };

            _mockService
                .Setup(s => s.UpdateNotificationAsync(1, ValidUpdateDto))
                .ReturnsAsync(NotificationsService<NotificationDto>
                    .GoodResult("Updated", 200, updatedDto));

            var result = await _controller.UpdateNotification(1, ValidUpdateDto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        [Fact]
        public async Task UpdateNotification_Returns404_WhenNotFound()
        {
            _mockService
                .Setup(s => s.UpdateNotificationAsync(99, ValidUpdateDto))
                .ReturnsAsync(NotificationsService<NotificationDto>
                    .BadResult("Notification with ID 99 not found", 404));

            var result = await _controller.UpdateNotification(99, ValidUpdateDto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        [Fact]
        public async Task UpdateNotification_Returns409_OnConcurrencyConflict()
        {
            _mockService
                .Setup(s => s.UpdateNotificationAsync(1, ValidUpdateDto))
                .ReturnsAsync(NotificationsService<NotificationDto>
                    .BadResult("Concurrency conflict", 409));

            var result = await _controller.UpdateNotification(1, ValidUpdateDto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(409, result!.StatusCode);
        }

        [Fact]
        public async Task UpdateNotification_Returns400_WhenModelInvalid()
        {
            InvalidateModel("Type", "Type is required");

            var result = await _controller.UpdateNotification(1, new UpdateNotificationDto()) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            _mockService.Verify(s => s.UpdateNotificationAsync(It.IsAny<int>(), It.IsAny<UpdateNotificationDto>()), Times.Never);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE /api/Notifications/{id}  →  IActionResult, rzutujemy wprost
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteNotification_Returns200_WhenSuccess()
        {
            _mockService
                .Setup(s => s.DeleteNotificationAsync(1))
                .ReturnsAsync(NotificationsService<bool>
                    .GoodResult("Deleted", 200, true));

            var result = await _controller.DeleteNotification(1) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
            Assert.Equal(true, result.Value);
        }

        [Fact]
        public async Task DeleteNotification_Returns404_WhenNotFound()
        {
            _mockService
                .Setup(s => s.DeleteNotificationAsync(99))
                .ReturnsAsync(NotificationsService<bool>
                    .BadResult("Notification with ID 99 not found", 404, data: false));

            var result = await _controller.DeleteNotification(99) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        [Fact]
        public async Task DeleteNotification_Returns500_WhenExceptionOccurs()
        {
            _mockService
                .Setup(s => s.DeleteNotificationAsync(1))
                .ReturnsAsync(NotificationsService<bool>
                    .BadResult("Failed to delete", 500, new List<string> { "DB error" }, false));

            var result = await _controller.DeleteNotification(1) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(500, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Weryfikacja liczby wywołań serwisu
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateNotification_CallsServiceExactlyOnce_WhenModelValid()
        {
            _mockService
                .Setup(s => s.CreateNotificationAsync(It.IsAny<CreateNotificationDto>()))
                .ReturnsAsync(NotificationsService<NotificationDto>
                    .GoodResult("Created", 201, SampleDto));

            await _controller.CreateNotification(ValidCreateDto);

            _mockService.Verify(s => s.CreateNotificationAsync(It.IsAny<CreateNotificationDto>()), Times.Once);
        }

        [Fact]
        public async Task DeleteNotification_CallsServiceExactlyOnce()
        {
            _mockService
                .Setup(s => s.DeleteNotificationAsync(It.IsAny<int>()))
                .ReturnsAsync(NotificationsService<bool>.GoodResult("Deleted", 200, true));

            await _controller.DeleteNotification(1);

            _mockService.Verify(s => s.DeleteNotificationAsync(1), Times.Once);
        }
    }
}