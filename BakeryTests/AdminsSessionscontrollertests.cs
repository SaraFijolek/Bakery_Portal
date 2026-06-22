using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;
using static WebApplication2.Properties.Services.AdminsSessionsService;

namespace WebApplication2.Tests.Controllers
{
    public class AdminsSessionsControllerTests
    {
        private readonly Mock<IAdminsSessionsService> _serviceMock;
        private readonly AdminsSessionsController _controller;

        public AdminsSessionsControllerTests()
        {
            _serviceMock = new Mock<IAdminsSessionsService>();
            _controller = new AdminsSessionsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAdminSessions_Returns200_WhenSuccessful()
        {
            // Arrange
            var sessions = new List<AdminSessionReadDto>
            {
                new AdminSessionReadDto
                {
                    SessionId = 1,
                    AdminId = "admin1",
                    SessionToken = "token123",
                    IsActive = true
                }
            };

            _serviceMock
                .Setup(x => x.GetAllAdminSessionsAsync())
                .ReturnsAsync(
                    AdminSessionService<List<AdminSessionReadDto>>
                        .GoodResult("OK", 200, sessions));

            // Act
            var result = await _controller.GetAdminSessions();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, objectResult.StatusCode);

            var returnedData =
                Assert.IsAssignableFrom<List<AdminSessionReadDto>>(objectResult.Value);

            Assert.Single(returnedData);
        }

        [Fact]
        public async Task GetAdminSession_Returns404_WhenNotFound()
        {
            // Arrange
            _serviceMock
                .Setup(x => x.GetAdminSessionByIdAsync(1))
                .ReturnsAsync(
                    AdminSessionService<AdminSessionReadDto>
                        .BadResult("Not found", 404));

            // Act
            var result = await _controller.GetAdminSession(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateAdminSession_Returns201_WhenCreated()
        {
            // Arrange
            var dto = new CreateAdminSessionDto
            {
                AdminId = "admin1",
                SessionToken = "token123",
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            };

            var createdDto = new AdminSessionReadDto
            {
                SessionId = 1,
                AdminId = dto.AdminId,
                SessionToken = dto.SessionToken
            };

            _serviceMock
                .Setup(x => x.CreateAdminSessionAsync(dto))
                .ReturnsAsync(
                    AdminSessionService<AdminSessionReadDto>
                        .GoodResult("Created", 201, createdDto));

            // Act
            var result = await _controller.CreateAdminSession(dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(201, objectResult.StatusCode);

            var value =
                Assert.IsType<AdminSessionReadDto>(objectResult.Value);

            Assert.Equal(1, value.SessionId);
        }

        [Fact]
        public async Task CreateAdminSession_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("AdminId", "Required");

            var dto = new CreateAdminSessionDto();

            // Act
            var result = await _controller.CreateAdminSession(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateAdminSession_Returns200_WhenUpdated()
        {
            // Arrange
            var dto = new UpdateAdminSessionDto
            {
                AdminId = "admin1",
                SessionToken = "newToken",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsActive = true
            };

            _serviceMock
                .Setup(x => x.UpdateAdminSessionAsync(1, dto))
                .ReturnsAsync(
                    AdminSessionService<bool>
                        .GoodResult("Updated", 200, true));

            // Act
            var result = await _controller.UpdateAdminSession(1, dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(true, objectResult.Value);
        }

        [Fact]
        public async Task UpdateAdminSession_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("SessionToken", "Required");

            var dto = new UpdateAdminSessionDto();

            // Act
            var result = await _controller.UpdateAdminSession(1, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAdminSession_Returns200_WhenDeleted()
        {
            // Arrange
            _serviceMock
                .Setup(x => x.DeleteAdminSessionAsync(1))
                .ReturnsAsync(
                    AdminSessionService<bool>
                        .GoodResult("Deleted", 200, true));

            // Act
            var result = await _controller.DeleteAdminSession(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(true, objectResult.Value);
        }

        [Fact]
        public async Task DeleteAdminSession_Returns404_WhenNotFound()
        {
            // Arrange
            _serviceMock
                .Setup(x => x.DeleteAdminSessionAsync(1))
                .ReturnsAsync(
                    AdminSessionService<bool>
                        .BadResult("Not found", 404));

            // Act
            var result = await _controller.DeleteAdminSession(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }
    }
}