using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;
using FluentAssertions;

namespace WebApplication2.Tests.Controllers
{
    public class UserSocialAuthControllerTests
    {
        private readonly Mock<IUserSocialAuthService> _serviceMock;
        private readonly UserSocialAuthController _controller;

        public UserSocialAuthControllerTests()
        {
            _serviceMock = new Mock<IUserSocialAuthService>();
            _controller = new UserSocialAuthController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetUserSocialAuths_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var data = new List<UserSocialAuthDto>
            {
                new UserSocialAuthDto
                {
                    SocialAuthId = 1,
                    UserId = "u1",
                    Provider = "Google"
                }
            };

            var serviceResult = UserSocialAuthsService<List<UserSocialAuthDto>>.GoodResult(
                "OK",
                200,
                data
            );

            _serviceMock
                .Setup(x => x.GetUserSocialAuthsAsync())
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUserSocialAuths();

            // Assert
            var obj = Assert.IsType<ObjectResult>(result.Result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetUserSocialAuth_ShouldReturnOk_WhenExists()
        {
            // Arrange
            var id = 1;

            var serviceResult = UserSocialAuthsService<UserSocialAuthDto>.GoodResult(
                "OK",
                200,
                new UserSocialAuthDto
                {
                    SocialAuthId = id,
                    UserId = "u1",
                    Provider = "Google"
                }
            );

            _serviceMock
                .Setup(x => x.GetUserSocialAuthByIdAsync(id))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUserSocialAuth(id);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result.Result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetUserSocialAuth_ShouldReturnNotFound_WhenMissing()
        {
            // Arrange
            var id = 999;

            var serviceResult = UserSocialAuthsService<UserSocialAuthDto>.BadResult(
                "Not found",
                404
            );

            _serviceMock
                .Setup(x => x.GetUserSocialAuthByIdAsync(id))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUserSocialAuth(id);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result.Result);
            obj.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreateUserSocialAuth_ShouldReturnCreated_WhenSuccess()
        {
            // Arrange
            var dto = new CreateUserSocialAuthDto
            {
                UserId = "u1",
                Provider = "Google",
                ProviderId = "google-123"
            };

            var serviceResult = UserSocialAuthsService<UserSocialAuthDto>.GoodResult(
                "Created",
                201,
                new UserSocialAuthDto
                {
                    SocialAuthId = 1,
                    UserId = dto.UserId,
                    Provider = dto.Provider,
                    ProviderId = dto.ProviderId
                }
            );

            _serviceMock
                .Setup(x => x.CreateUserSocialAuthAsync(dto))
                .ReturnsAsync(serviceResult);

            _controller.ModelState.Clear();

            // Act
            var result = await _controller.CreateUserSocialAuth(dto);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result.Result);
            obj.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task CreateUserSocialAuth_ShouldReturnBadRequest_WhenModelInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserId", "Required");

            var dto = new CreateUserSocialAuthDto();

            // Act
            var result = await _controller.CreateUserSocialAuth(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            badRequest.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdateUserSocialAuth_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var id = 1;

            var dto = new UpdateUserSocialAuthDto
            {
                UserId = "u1",
                Provider = "Google",
                ProviderId = "google-123"
            };

            var serviceResult = UserSocialAuthsService<UserSocialAuthDto>.GoodResult(
                "Updated",
                200,
                new UserSocialAuthDto
                {
                    SocialAuthId = id,
                    UserId = dto.UserId,
                    Provider = dto.Provider
                }
            );

            _serviceMock
                .Setup(x => x.UpdateUserSocialAuthAsync(id, dto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdateUserSocialAuth(id, dto);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteUserSocialAuth_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var id = 1;

            var serviceResult = UserSocialAuthsService<bool>.GoodResult(
                "Deleted",
                200,
                true
            );

            _serviceMock
                .Setup(x => x.DeleteUserSocialAuthAsync(id))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.DeleteUserSocialAuth(id);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteUserSocialAuth_ShouldReturnForbidden_WhenNotAdmin()
        {
            // Arrange
            var id = 1;

            var serviceResult = UserSocialAuthsService<bool>.BadResult(
                "Forbidden",
                403,
                data: false
            );

            _serviceMock
                .Setup(x => x.DeleteUserSocialAuthAsync(id))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.DeleteUserSocialAuth(id);

            // Assert
            var obj = Assert.IsType<ObjectResult>(result);
            obj.StatusCode.Should().Be(403);
        }
    }
}
