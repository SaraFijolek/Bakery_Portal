using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Backery.API.Controllers;
using WebApplication2.Backery.API.DTO;
using WebApplication2.Backery.Services.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class StatisticsControllerTests
    {
        private readonly Mock<IStatisticsService> _mockService;
        private readonly StatisticsController _controller;

        private static readonly StatisticsDto SampleStats = new()
        {
            TotalUsers = 100,
            TotalAds = 50,
            ActiveAds = 30,
            TotalComments = 200,
            AverageRating = 4.2,
            NewAdsLast7Days = 5
        };

        public StatisticsControllerTests()
        {
            _mockService = new Mock<IStatisticsService>();
            _controller = new StatisticsController(_mockService.Object);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/statistics
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public void Get_Returns200_WithStatistics()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetStatistics())
                .Returns(SampleStats);

            // Act
            var result = _controller.Get() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
            Assert.Equal(SampleStats, result.Value);
        }

        [Fact]
        public void Get_ReturnsOkObjectResult()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetStatistics())
                .Returns(SampleStats);

            // Act
            var result = _controller.Get();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Get_ReturnsCorrectTotalUsers()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetStatistics())
                .Returns(SampleStats);

            // Act
            var result = _controller.Get() as OkObjectResult;
            var data = result?.Value as StatisticsDto;

            // Assert
            Assert.NotNull(data);
            Assert.Equal(100, data!.TotalUsers);
        }

        [Fact]
        public void Get_ReturnsCorrectTotalAds()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetStatistics())
                .Returns(SampleStats);

            // Act
            var result = _controller.Get() as OkObjectResult;
            var data = result?.Value as StatisticsDto;

            // Assert
            Assert.NotNull(data);
            Assert.Equal(50, data!.TotalAds);
        }

        [Fact]
        public void Get_ActiveAds_LessOrEqualTotalAds()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetStatistics())
                .Returns(SampleStats);

            // Act
            var result = _controller.Get() as OkObjectResult;
            var data = result?.Value as StatisticsDto;

            // Assert
            Assert.NotNull(data);
            Assert.True(data!.ActiveAds <= data.TotalAds);
        }

        [Fact]
        public void Get_ReturnsZeroAverageRating_WhenNoRatings()
        {
            // Arrange
            var emptyStats = new StatisticsDto
            {
                TotalUsers = 10,
                TotalAds = 5,
                ActiveAds = 2,
                TotalComments = 0,
                AverageRating = 0,
                NewAdsLast7Days = 1
            };
            _mockService
                .Setup(s => s.GetStatistics())
                .Returns(emptyStats);

            // Act
            var result = _controller.Get() as OkObjectResult;
            var data = result?.Value as StatisticsDto;

            // Assert
            Assert.NotNull(data);
            Assert.Equal(0, data!.AverageRating);
        }

        [Fact]
        public void Get_ReturnsAllZeros_WhenDatabaseEmpty()
        {
            // Arrange
            var zeroStats = new StatisticsDto
            {
                TotalUsers = 0,
                TotalAds = 0,
                ActiveAds = 0,
                TotalComments = 0,
                AverageRating = 0,
                NewAdsLast7Days = 0
            };
            _mockService
                .Setup(s => s.GetStatistics())
                .Returns(zeroStats);

            // Act
            var result = _controller.Get() as OkObjectResult;
            var data = result?.Value as StatisticsDto;

            // Assert
            Assert.NotNull(data);
            Assert.Equal(0, data!.TotalUsers);
            Assert.Equal(0, data.TotalAds);
            Assert.Equal(0, data.ActiveAds);
            Assert.Equal(0, data.TotalComments);
            Assert.Equal(0, data.AverageRating);
            Assert.Equal(0, data.NewAdsLast7Days);
        }

        [Fact]
        public void Get_ReturnsCorrectNewAdsLast7Days()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetStatistics())
                .Returns(SampleStats);

            // Act
            var result = _controller.Get() as OkObjectResult;
            var data = result?.Value as StatisticsDto;

            // Assert
            Assert.NotNull(data);
            Assert.Equal(5, data!.NewAdsLast7Days);
        }

        [Fact]
        public void Get_CallsServiceExactlyOnce()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetStatistics())
                .Returns(SampleStats);

            // Act
            _controller.Get();

            // Assert
            _mockService.Verify(s => s.GetStatistics(), Times.Once);
        }

        [Fact]
        public void Get_NeverCallsService_WhenNotInvoked()
        {
            // Assert
            _mockService.Verify(s => s.GetStatistics(), Times.Never);
        }

        [Fact]
        public void Get_ReturnsCorrectAverageRating()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetStatistics())
                .Returns(SampleStats);

            // Act
            var result = _controller.Get() as OkObjectResult;
            var data = result?.Value as StatisticsDto;

            // Assert
            Assert.NotNull(data);
            Assert.Equal(4.2, data!.AverageRating, precision: 1);
        }
    }
}