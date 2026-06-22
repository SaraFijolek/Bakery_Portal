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
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ILogger<UsersController>> _loggerMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<UsersController>>();

            _controller = new UsersController(
                _userServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetUser_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var userId = "123";

            var serviceResult = UsersService<UserResponseDto>.GoodResult(
                "OK",
                (int)HttpStatusCode.OK,
                new UserResponseDto
                {
                    UserId = userId,
                    Name = "Test",
                    Email = "test@test.com"
                });

            _userServiceMock
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetUser_ShouldReturnBadRequest_WhenIdIsEmpty()
        {
            // Act
            var result = await _controller.GetUser("");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            badRequest.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreated_WhenSuccess()
        {
            // Arrange
            var dto = new UserCreateDto
            {
                Name = "Test",
                Email = "test@test.com",
                Password = "password123"
            };

            var serviceResult = UsersService<UserResponseDto>.GoodResult(
                "Created",
                (int)HttpStatusCode.Created,
                new UserResponseDto { UserId = "1", Name = "Test", Email = "test@test.com" }
            );

            _userServiceMock
                .Setup(x => x.CreateUserAsync(dto))
                .ReturnsAsync(serviceResult);

            _controller.ModelState.Clear(); // ważne

            // Act
            var result = await _controller.CreateUser(dto);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenModelInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");

            var dto = new UserCreateDto();

            // Act
            var result = await _controller.CreateUser(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            badRequest.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var id = "1";

            var serviceResult = UsersService<bool>.GoodResult(
                "Deleted",
                (int)HttpStatusCode.OK,
                true
            );

            _userServiceMock
                .Setup(x => x.DeleteUserAsync(id))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.DeleteUser(id);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdateUserStatus_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var id = "1";

            var serviceResult = UsersService<bool>.GoodResult(
                "Updated",
                (int)HttpStatusCode.OK,
                true
            );

            _userServiceMock
                .Setup(x => x.UpdateUserStatusAsync(id, true))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdateUserStatus(id, true);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOk()
        {
            // Arrange
            var query = new UserQueryDto();

            var serviceResult = UsersService<PagedResult<UserResponseDto>>.GoodResult(
                "OK",
                (int)HttpStatusCode.OK,
                new PagedResult<UserResponseDto>
                {
                    Items = new List<UserResponseDto>(),
                    TotalCount = 0,
                    Page = 1,
                    PageSize = 10
                }
            );

            _userServiceMock
                .Setup(x => x.GetUsersAsync(It.IsAny<UserQueryDto>()))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUsers(query);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }
    }
}