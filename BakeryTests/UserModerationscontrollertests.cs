using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;
using FluentAssertions;

namespace WebApplication2.Tests.Controllers
{
    public class UserModerationsControllerTests
    {
        private readonly Mock<IUserModerationsService> _serviceMock;
        private readonly UserModerationsController _controller;

        public UserModerationsControllerTests()
        {
            _serviceMock = new Mock<IUserModerationsService>();

            _controller = new UserModerationsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetUserModerations_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var data = new List<UserModerationDto>
            {
                new UserModerationDto
                {
                    ModerationId = 1,
                    UserId = "u1",
                    AdminId = "a1",
                    Action = "ban"
                }
            };

            var serviceResult = UserModerationService<List<UserModerationDto>>.GoodResult(
                "OK",
                200,
                data
            );

            _serviceMock
                .Setup(x => x.GetUserModerationsAsync())
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUserModerations();

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetUserModerationsList_ShouldReturnOk()
        {
            // Arrange
            var data = new List<UserModerationListDto>();

            var serviceResult = UserModerationService<List<UserModerationListDto>>.GoodResult(
                "OK",
                200,
                data
            );

            _serviceMock
                .Setup(x => x.GetUserModerationsListAsync())
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUserModerationsList();

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetActiveUserModerations_ShouldReturnOk()
        {
            // Arrange
            var serviceResult = UserModerationService<List<UserModerationDto>>.GoodResult(
                "OK",
                200,
                new List<UserModerationDto>()
            );

            _serviceMock
                .Setup(x => x.GetActiveUserModerationsAsync())
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetActiveUserModerations();

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetUserModerationsByUserId_ShouldReturnOk()
        {
            // Arrange
            var userId = "u1";

            var serviceResult = UserModerationService<List<UserModerationDto>>.GoodResult(
                "OK",
                200,
                new List<UserModerationDto>()
            );

            _serviceMock
                .Setup(x => x.GetUserModerationsByUserIdAsync(userId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUserModerationsByUserId(userId);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetUserModeration_ShouldReturnOk()
        {
            // Arrange
            var id = 1;

            var serviceResult = UserModerationService<UserModerationDto>.GoodResult(
                "OK",
                200,
                new UserModerationDto { ModerationId = id }
            );

            _serviceMock
                .Setup(x => x.GetUserModerationByIdAsync(id))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUserModeration(id);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task CreateUserModeration_ShouldReturnCreated_WhenSuccess()
        {
            // Arrange
            var dto = new CreateUserModerationDto
            {
                UserId = "u1",
                AdminId = "a1",
                Action = "ban"
            };

            var serviceResult = UserModerationService<UserModerationDto>.GoodResult(
                "Created",
                201,
                new UserModerationDto
                {
                    ModerationId = 1,
                    UserId = dto.UserId,
                    AdminId = dto.AdminId,
                    Action = dto.Action
                }
            );

            _serviceMock
                .Setup(x => x.CreateUserModerationAsync(dto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CreateUserModeration(dto);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task UpdateUserModeration_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var id = 1;

            var dto = new UpdateUserModerationDto
            {
                UserId = "u1",
                AdminId = "a1",
                Action = "warn",
                IsActive = true
            };

            var serviceResult = UserModerationService<bool>.GoodResult(
                "Updated",
                200,
                true
            );

            _serviceMock
                .Setup(x => x.UpdateUserModerationAsync(id, dto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdateUserModeration(id, dto);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteUserModeration_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var id = 1;

            var serviceResult = UserModerationService<bool>.GoodResult(
                "Deleted",
                200,
                true
            );

            _serviceMock
                .Setup(x => x.DeleteUserModerationAsync(id))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.DeleteUserModeration(id);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }
    }
}