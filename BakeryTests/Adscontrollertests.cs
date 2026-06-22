using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class AdsControllerTests
    {
        private readonly Mock<IAdsService> _serviceMock;
        private readonly AdsController _controller;

        public AdsControllerTests()
        {
            _serviceMock = new Mock<IAdsService>();
            _controller = new AdsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAds_Returns200_WhenSuccessful()
        {
            // Arrange
            var ads = new List<AdResponseDto>
            {
                new()
                {
                    AdId = 1,
                    UserId = "user1",
                    Title = "BMW",
                    SubcategoryId = 1
                }
            };

            _serviceMock
                .Setup(x => x.GetAllAdsAsync())
                .ReturnsAsync(
                    AdService<IEnumerable<AdResponseDto>>
                        .GoodResult("Success", 200, ads));

            // Act
            var result = await _controller.GetAds();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);

            var value =
                Assert.IsAssignableFrom<IEnumerable<AdResponseDto>>(objectResult.Value);

            Assert.Single(value);
        }

        [Fact]
        public async Task GetAd_Returns200_WhenFound()
        {
            // Arrange
            var ad = new AdResponseDto
            {
                AdId = 1,
                UserId = "user1",
                Title = "BMW",
                SubcategoryId = 1
            };

            _serviceMock
                .Setup(x => x.GetAdByIdAsync(1))
                .ReturnsAsync(
                    AdService<AdResponseDto>
                        .GoodResult("Success", 200, ad));

            // Act
            var result = await _controller.GetAd(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);

            var value = Assert.IsType<AdResponseDto>(objectResult.Value);
            Assert.Equal(1, value.AdId);
        }

        [Fact]
        public async Task GetAd_Returns404_WhenNotFound()
        {
            // Arrange
            _serviceMock
                .Setup(x => x.GetAdByIdAsync(999))
                .ReturnsAsync(
                    AdService<AdResponseDto>
                        .BadResult("Not found", 404));

            // Act
            var result = await _controller.GetAd(999);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateAd_Returns201_WhenCreated()
        {
            // Arrange
            var createDto = new AdCreateDto
            {
                UserId = "user1",
                SubcategoryId = 1,
                Title = "BMW"
            };

            var createdAd = new AdResponseDto
            {
                AdId = 1,
                UserId = "user1",
                SubcategoryId = 1,
                Title = "BMW"
            };

            _serviceMock
                .Setup(x => x.CreateAdAsync(createDto))
                .ReturnsAsync(
                    AdService<AdResponseDto>
                        .GoodResult("Created", 201, createdAd));

            // Act
            var result = await _controller.CreateAd(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);

            var value = Assert.IsType<AdResponseDto>(objectResult.Value);
            Assert.Equal(1, value.AdId);
        }

        [Fact]
        public async Task UpdateAd_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var dto = new AdUpdateDto
            {
                AdId = 2,
                UserId = "user1",
                SubcategoryId = 1,
                Title = "BMW"
            };

            // Act
            var result = await _controller.UpdateAd(1, dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task UpdateAd_Returns200_WhenUpdated()
        {
            // Arrange
            var dto = new AdUpdateDto
            {
                AdId = 1,
                UserId = "user1",
                SubcategoryId = 1,
                Title = "Audi"
            };

            _serviceMock
                .Setup(x => x.UpdateAdAsync(dto))
                .ReturnsAsync(
                    AdService<bool>
                        .GoodResult("Updated", 200, true));

            // Act
            var result = await _controller.UpdateAd(1, dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(true, objectResult.Value);
        }

        [Fact]
        public async Task UpdateAd_Returns404_WhenAdNotFound()
        {
            // Arrange
            var dto = new AdUpdateDto
            {
                AdId = 1,
                UserId = "user1",
                SubcategoryId = 1,
                Title = "Audi"
            };

            _serviceMock
                .Setup(x => x.UpdateAdAsync(dto))
                .ReturnsAsync(
                    AdService<bool>
                        .BadResult("Not found", 404));

            // Act
            var result = await _controller.UpdateAd(1, dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteAd_Returns200_WhenDeleted()
        {
            // Arrange
            _serviceMock
                .Setup(x => x.DeleteAdAsync(1))
                .ReturnsAsync(
                    AdService<bool>
                        .GoodResult("Deleted", 200, true));

            // Act
            var result = await _controller.DeleteAd(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteAd_Returns404_WhenNotFound()
        {
            // Arrange
            _serviceMock
                .Setup(x => x.DeleteAdAsync(999))
                .ReturnsAsync(
                    AdService<bool>
                        .BadResult("Not found", 404));

            // Act
            var result = await _controller.DeleteAd(999);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, objectResult.StatusCode);
        }
    }
}