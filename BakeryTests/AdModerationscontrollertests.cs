using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class AdModerationsControllerTests
    {
        private readonly Mock<IAdModerationsService> _serviceMock;
        private readonly AdModerationsController _controller;

        public AdModerationsControllerTests()
        {
            _serviceMock = new Mock<IAdModerationsService>();
            _controller = new AdModerationsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAdModerations_Returns200_WhenSuccessful()
        {
            // Arrange
            var moderations = new List<AdModerationDto>
            {
                new AdModerationDto
                {
                    ModerationId = 1,
                    AdId = 10,
                    Action = "Approved",
                    Reason = "OK"
                }
            };

            _serviceMock
                .Setup(x => x.GetAdModerationsAsync())
                .ReturnsAsync(
                    AdModerationService<IEnumerable<AdModerationDto>>
                        .GoodResult("Success", 200, moderations));

            // Act
            var result = await _controller.GetAdModerations();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, objectResult.StatusCode);

            var value =
                Assert.IsAssignableFrom<IEnumerable<AdModerationDto>>(objectResult.Value);

            Assert.Single(value);
        }

        [Fact]
        public async Task GetAdModeration_Returns200_WhenFound()
        {
            // Arrange
            var moderation = new AdModerationDto
            {
                ModerationId = 1,
                AdId = 10,
                Action = "Approved"
            };

            _serviceMock
                .Setup(x => x.GetAdModerationByIdAsync(1))
                .ReturnsAsync(
                    AdModerationService<AdModerationDto>
                        .GoodResult("Success", 200, moderation));

            // Act
            var result = await _controller.GetAdModeration(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, objectResult.StatusCode);

            var value = Assert.IsType<AdModerationDto>(objectResult.Value);
            Assert.Equal(1, value.ModerationId);
        }

        [Fact]
        public async Task GetAdModeration_Returns404_WhenNotFound()
        {
            // Arrange
            _serviceMock
                .Setup(x => x.GetAdModerationByIdAsync(999))
                .ReturnsAsync(
                    AdModerationService<AdModerationDto>
                        .BadResult("Not found", 404));

            // Act
            var result = await _controller.GetAdModeration(999);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateAdModeration_Returns201_WhenCreated()
        {
            // Arrange
            var createDto = new CreateAdModerationDto
            {
                AdId = 10,
                AdminId = "1",
                Action = "Approved",
                Reason = "Looks good"
            };

            var created = new AdModerationDto
            {
                ModerationId = 1,
                AdId = 10,
                Action = "Approved",
                Reason = "Looks good"
            };

            _serviceMock
                .Setup(x => x.CreateAdModerationAsync(createDto))
                .ReturnsAsync(
                    AdModerationService<AdModerationDto>
                        .GoodResult("Created", 201, created));

            // Act
            var result = await _controller.CreateAdModeration(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(201, objectResult.StatusCode);

            var value = Assert.IsType<AdModerationDto>(objectResult.Value);
            Assert.Equal(1, value.ModerationId);
        }

        [Fact]
        public async Task CreateAdModeration_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Action", "Required");

            var dto = new CreateAdModerationDto();

            // Act
            var result = await _controller.CreateAdModeration(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateAdModeration_Returns200_WhenUpdated()
        {
            // Arrange
            var updateDto = new UpdateAdModerationDto
            {
                AdId = 10,
                AdminId = "1",
                Action = "Rejected",
                Reason = "Spam"
            };

            var updated = new AdModerationDto
            {
                ModerationId = 1,
                AdId = 10,
                Action = "Rejected",
                Reason = "Spam"
            };

            _serviceMock
                .Setup(x => x.UpdateAdModerationAsync(1, updateDto))
                .ReturnsAsync(
                    AdModerationService<AdModerationDto>
                        .GoodResult("Updated", 200, updated));

            // Act
            var result = await _controller.UpdateAdModeration(1, updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, objectResult.StatusCode);

            var value = Assert.IsType<AdModerationDto>(objectResult.Value);
            Assert.Equal("Rejected", value.Action);
        }

        [Fact]
        public async Task UpdateAdModeration_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Action", "Required");

            var dto = new UpdateAdModerationDto();

            // Act
            var result = await _controller.UpdateAdModeration(1, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteAdModeration_Returns200_WhenDeleted()
        {
            // Arrange
            _serviceMock
                .Setup(x => x.DeleteAdModerationAsync(1))
                .ReturnsAsync(
                    AdModerationService<bool>
                        .GoodResult("Deleted", 200, true));

            // Act
            var result = await _controller.DeleteAdModeration(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(true, objectResult.Value);
        }

        [Fact]
        public async Task DeleteAdModeration_Returns404_WhenNotFound()
        {
            // Arrange
            _serviceMock
                .Setup(x => x.DeleteAdModerationAsync(999))
                .ReturnsAsync(
                    AdModerationService<bool>
                        .BadResult("Not found", 404));

            // Act
            var result = await _controller.DeleteAdModeration(999);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }
    }
}