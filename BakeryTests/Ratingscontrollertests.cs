using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class RatingsControllerTests
    {
        private readonly Mock<IRatingsService> _mockService;
        private readonly RatingsController _controller;

        // ── Przykładowe dane ──────────────────────────────────────────────────
        private static readonly RatingDto SampleDto = new()
        {
            RatingId = 1,
            FromUserId = "user-111",
            ToUserId = "user-222",
            Score = 4,
            CreatedAt = new DateTime(2024, 1, 1),
            FromUserName = "Alice",
            ToUserName = "Bob"
        };

        private static readonly CreateRatingDto ValidCreateDto = new()
        {
            FromUserId = "user-111",
            ToUserId = "user-222",
            Score = 4
        };

        private static readonly UpdateRatingDto ValidUpdateDto = new()
        {
            FromUserId = "user-111",
            ToUserId = "user-222",
            Score = 5
        };

        // ── Konstruktor ───────────────────────────────────────────────────────
        public RatingsControllerTests()
        {
            _mockService = new Mock<IRatingsService>();
            _controller = new RatingsController(_mockService.Object);
        }

        // ── Helper ────────────────────────────────────────────────────────────
        private void InvalidateModel(string field, string error)
            => _controller.ModelState.AddModelError(field, error);

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/Ratings
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetRatings_Returns200_WithData_WhenSuccess()
        {
            // Arrange
            var list = new List<RatingDto> { SampleDto };
            _mockService
                .Setup(s => s.GetRatingsAsync())
                .ReturnsAsync(RatingService<IEnumerable<RatingDto>>
                    .GoodResult("OK", 200, list));

            // Act
            var result = await _controller.GetRatings() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
            Assert.Equal(list, result.Value);
        }

        [Fact]
        public async Task GetRatings_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetRatingsAsync())
                .ReturnsAsync(RatingService<IEnumerable<RatingDto>>
                    .BadResult("Internal error", 500, new List<string> { "DB error" }));

            // Act
            var result = await _controller.GetRatings() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/Ratings/{id}
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetRating_Returns200_WhenFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetRatingByIdAsync(1))
                .ReturnsAsync(RatingService<RatingDto>
                    .GoodResult("OK", 200, SampleDto));

            // Act
            var result = await _controller.GetRating(1) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
            Assert.Equal(SampleDto, result.Value);
        }

        [Fact]
        public async Task GetRating_Returns404_WhenNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetRatingByIdAsync(99))
                .ReturnsAsync(RatingService<RatingDto>
                    .BadResult("Rating with ID 99 not found", 404));

            // Act
            var result = await _controller.GetRating(99) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/Ratings
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateRating_Returns201_WhenValid()
        {
            // Arrange
            _mockService
                .Setup(s => s.CreateRatingAsync(ValidCreateDto))
                .ReturnsAsync(RatingService<RatingDto>
                    .GoodResult("Created", 201, SampleDto));

            // Act
            var result = await _controller.CreateRating(ValidCreateDto) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result!.StatusCode);
            Assert.Equal(SampleDto, result.Value);
        }

        [Fact]
        public async Task CreateRating_Returns400_WhenModelInvalid()
        {
            // Arrange – brakujące pole FromUserId
            InvalidateModel("FromUserId", "FromUserId is required");

            // Act
            var result = await _controller.CreateRating(new CreateRatingDto()) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            _mockService.Verify(s => s.CreateRatingAsync(It.IsAny<CreateRatingDto>()), Times.Never);
        }

        [Fact]
        public async Task CreateRating_Returns400_WhenUsersSame()
        {
            // Arrange – serwis zwraca błąd "cannot rate themselves"
            var sameUserDto = new CreateRatingDto
            {
                FromUserId = "user-111",
                ToUserId = "user-111",
                Score = 3
            };
            _mockService
                .Setup(s => s.CreateRatingAsync(sameUserDto))
                .ReturnsAsync(RatingService<RatingDto>
                    .BadResult("Validation failed", 400,
                        new List<string> { "User cannot rate themselves" }));

            // Act
            var result = await _controller.CreateRating(sameUserDto) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
        }

        [Fact]
        public async Task CreateRating_Returns400_WhenUserNotExist()
        {
            // Arrange
            _mockService
                .Setup(s => s.CreateRatingAsync(ValidCreateDto))
                .ReturnsAsync(RatingService<RatingDto>
                    .BadResult("Validation failed", 400,
                        new List<string> { "FromUser with ID user-111 does not exist" }));

            // Act
            var result = await _controller.CreateRating(ValidCreateDto) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // PUT /api/Ratings/{id}
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateRating_Returns200_WhenSuccess()
        {
            // Arrange
            var updatedDto = new RatingDto
            {
                RatingId = SampleDto.RatingId,
                FromUserId = SampleDto.FromUserId,
                ToUserId = SampleDto.ToUserId,
                Score = 5,   // zmieniona wartość
                CreatedAt = SampleDto.CreatedAt,
                FromUserName = SampleDto.FromUserName,
                ToUserName = SampleDto.ToUserName
            };
            _mockService
                .Setup(s => s.UpdateRatingAsync(1, ValidUpdateDto))
                .ReturnsAsync(RatingService<RatingDto>
                    .GoodResult("Updated", 200, updatedDto));

            // Act
            var result = await _controller.UpdateRating(1, ValidUpdateDto) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        [Fact]
        public async Task UpdateRating_Returns404_WhenNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.UpdateRatingAsync(99, ValidUpdateDto))
                .ReturnsAsync(RatingService<RatingDto>
                    .BadResult("Rating with ID 99 not found", 404));

            // Act
            var result = await _controller.UpdateRating(99, ValidUpdateDto) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        [Fact]
        public async Task UpdateRating_Returns409_OnConcurrencyConflict()
        {
            // Arrange
            _mockService
                .Setup(s => s.UpdateRatingAsync(1, ValidUpdateDto))
                .ReturnsAsync(RatingService<RatingDto>
                    .BadResult("Concurrency conflict occurred while updating rating", 409));

            // Act
            var result = await _controller.UpdateRating(1, ValidUpdateDto) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(409, result!.StatusCode);
        }

        [Fact]
        public async Task UpdateRating_Returns400_WhenModelInvalid()
        {
            // Arrange
            InvalidateModel("Score", "Score must be between 1 and 5");

            // Act
            var result = await _controller.UpdateRating(1, new UpdateRatingDto()) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            _mockService.Verify(
                s => s.UpdateRatingAsync(It.IsAny<int>(), It.IsAny<UpdateRatingDto>()),
                Times.Never);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE /api/Ratings/{id}
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteRating_Returns200_WhenSuccess()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteRatingAsync(1))
                .ReturnsAsync(RatingService<bool>
                    .GoodResult("Deleted", 200, true));

            // Act
            var result = await _controller.DeleteRating(1) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
            Assert.Equal(true, result.Value);
        }

        [Fact]
        public async Task DeleteRating_Returns404_WhenNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteRatingAsync(99))
                .ReturnsAsync(RatingService<bool>
                    .BadResult("Rating with ID 99 not found", 404, data: false));

            // Act
            var result = await _controller.DeleteRating(99) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        [Fact]
        public async Task DeleteRating_Returns500_WhenExceptionOccurs()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteRatingAsync(1))
                .ReturnsAsync(RatingService<bool>
                    .BadResult("Failed to delete rating", 500,
                        new List<string> { "DB error" }, false));

            // Act
            var result = await _controller.DeleteRating(1) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Weryfikacja liczby wywołań serwisu
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateRating_CallsServiceExactlyOnce_WhenModelValid()
        {
            // Arrange
            _mockService
                .Setup(s => s.CreateRatingAsync(It.IsAny<CreateRatingDto>()))
                .ReturnsAsync(RatingService<RatingDto>
                    .GoodResult("Created", 201, SampleDto));

            // Act
            await _controller.CreateRating(ValidCreateDto);

            // Assert
            _mockService.Verify(
                s => s.CreateRatingAsync(It.IsAny<CreateRatingDto>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteRating_CallsServiceExactlyOnce()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteRatingAsync(It.IsAny<int>()))
                .ReturnsAsync(RatingService<bool>.GoodResult("Deleted", 200, true));

            // Act
            await _controller.DeleteRating(1);

            // Assert
            _mockService.Verify(s => s.DeleteRatingAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpdateRating_CallsServiceExactlyOnce_WhenModelValid()
        {
            // Arrange
            _mockService
                .Setup(s => s.UpdateRatingAsync(It.IsAny<int>(), It.IsAny<UpdateRatingDto>()))
                .ReturnsAsync(RatingService<RatingDto>
                    .GoodResult("Updated", 200, SampleDto));

            // Act
            await _controller.UpdateRating(1, ValidUpdateDto);

            // Assert
            _mockService.Verify(
                s => s.UpdateRatingAsync(1, It.IsAny<UpdateRatingDto>()),
                Times.Once);
        }
    }
}