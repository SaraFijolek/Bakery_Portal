using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class ReportedContentsControllerTests
    {
        private readonly Mock<IReportedContentsService> _mockService;
        private readonly ReportedContentsController _controller;

        // ── Przykładowe dane ──────────────────────────────────────────────────
        private static readonly ReportedContentReadDto SampleDto = new()
        {
            ReportId = 1,
            ReporterUserId = "user-123",
            ReporterEmail = "user@example.com",
            ContentType = "Ad",
            ContentId = 10,
            Reason = "Spam",
            Description = "This is spam content",
            Status = "pending",
            AdminId = null,
            AdminNotes = "",
            CreatedAt = new DateTime(2024, 1, 1),
            ResolvedAt = null
        };

        private static readonly CreateReportedContentDto ValidCreateDto = new()
        {
            ReporterUserId = "user-123",
            ReporterEmail = "user@example.com",
            ContentType = "Ad",
            ContentId = 10,
            Reason = "Spam",
            Description = "This is spam content",
            Status = "pending"
        };

        private static readonly UpdateReportedContentDto ValidUpdateDto = new()
        {
            ReporterUserId = "user-123",
            ReporterEmail = "user@example.com",
            ContentType = "Ad",
            ContentId = 10,
            Reason = "Spam",
            Description = "Updated description",
            Status = "resolved",
            AdminNotes = "Reviewed and resolved"
        };

        // ── Konstruktor ───────────────────────────────────────────────────────
        public ReportedContentsControllerTests()
        {
            _mockService = new Mock<IReportedContentsService>();
            _controller = new ReportedContentsController(_mockService.Object);
        }

        // ── Helper ────────────────────────────────────────────────────────────
        private void InvalidateModel(string field, string error)
            => _controller.ModelState.AddModelError(field, error);

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/ReportedContents  →  ActionResult<IEnumerable<T>>, używamy .Result
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetReportedContents_Returns200_WithData_WhenSuccess()
        {
            // Arrange
            var list = new List<ReportedContentReadDto> { SampleDto };
            _mockService
                .Setup(s => s.GetReportedContentsAsync())
                .ReturnsAsync(ReportedContentService<IEnumerable<ReportedContentReadDto>>
                    .GoodResult("OK", 200, list));

            // Act
            var actionResult = await _controller.GetReportedContents();
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
            Assert.Equal(list, result.Value);
        }

        [Fact]
        public async Task GetReportedContents_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetReportedContentsAsync())
                .ReturnsAsync(ReportedContentService<IEnumerable<ReportedContentReadDto>>
                    .BadResult("Internal error", 500, new List<string> { "DB error" }));

            // Act
            var actionResult = await _controller.GetReportedContents();
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/ReportedContents/{id}  →  ActionResult<T>, używamy .Result
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetReportedContent_Returns200_WhenFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetReportedContentAsync(1))
                .ReturnsAsync(ReportedContentService<ReportedContentReadDto>
                    .GoodResult("OK", 200, SampleDto));

            // Act
            var actionResult = await _controller.GetReportedContent(1);
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
            Assert.Equal(SampleDto, result.Value);
        }

        [Fact]
        public async Task GetReportedContent_Returns404_WhenNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetReportedContentAsync(99))
                .ReturnsAsync(ReportedContentService<ReportedContentReadDto>
                    .BadResult("Reported content with ID 99 not found", 404));

            // Act
            var actionResult = await _controller.GetReportedContent(99);
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/ReportedContents  →  ActionResult<T>, używamy .Result
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateReportedContent_Returns201_WhenValid()
        {
            // Arrange
            _mockService
                .Setup(s => s.CreateReportedContentAsync(ValidCreateDto))
                .ReturnsAsync(ReportedContentService<ReportedContentReadDto>
                    .GoodResult("Created", 201, SampleDto));

            // Act
            var actionResult = await _controller.CreateReportedContent(ValidCreateDto);
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result!.StatusCode);
            Assert.Equal(SampleDto, result.Value);
        }

        [Fact]
        public async Task CreateReportedContent_Returns400_WhenModelInvalid()
        {
            // Arrange – brakujące wymagane pole ContentType
            InvalidateModel("ContentType", "ContentType is required");

            // Act
            var actionResult = await _controller.CreateReportedContent(new CreateReportedContentDto());
            var result = actionResult.Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            _mockService.Verify(
                s => s.CreateReportedContentAsync(It.IsAny<CreateReportedContentDto>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateReportedContent_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.CreateReportedContentAsync(ValidCreateDto))
                .ReturnsAsync(ReportedContentService<ReportedContentReadDto>
                    .BadResult("Failed to create reported content", 500,
                        new List<string> { "DB error" }));

            // Act
            var actionResult = await _controller.CreateReportedContent(ValidCreateDto);
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // PUT /api/ReportedContents/{id}  →  ActionResult<T>, używamy .Result
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateReportedContent_Returns200_WhenSuccess()
        {
            // Arrange
            var updatedDto = new ReportedContentReadDto
            {
                ReportId = SampleDto.ReportId,
                ReporterUserId = SampleDto.ReporterUserId,
                ReporterEmail = SampleDto.ReporterEmail,
                ContentType = SampleDto.ContentType,
                ContentId = SampleDto.ContentId,
                Reason = SampleDto.Reason,
                Description = "Updated description",
                Status = "resolved",
                AdminNotes = "Reviewed and resolved",
                CreatedAt = SampleDto.CreatedAt,
                ResolvedAt = DateTime.Now
            };

            _mockService
                .Setup(s => s.UpdateReportedContentAsync(1, ValidUpdateDto))
                .ReturnsAsync(ReportedContentService<ReportedContentReadDto>
                    .GoodResult("Updated", 200, updatedDto));

            // Act
            var actionResult = await _controller.UpdateReportedContent(1, ValidUpdateDto);
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        [Fact]
        public async Task UpdateReportedContent_Returns404_WhenNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.UpdateReportedContentAsync(99, ValidUpdateDto))
                .ReturnsAsync(ReportedContentService<ReportedContentReadDto>
                    .BadResult("Reported content with ID 99 not found", 404));

            // Act
            var actionResult = await _controller.UpdateReportedContent(99, ValidUpdateDto);
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        [Fact]
        public async Task UpdateReportedContent_Returns400_WhenModelInvalid()
        {
            // Arrange
            InvalidateModel("Status", "Status is required");

            // Act
            var actionResult = await _controller.UpdateReportedContent(1, new UpdateReportedContentDto());
            var result = actionResult.Result as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            _mockService.Verify(
                s => s.UpdateReportedContentAsync(It.IsAny<int>(), It.IsAny<UpdateReportedContentDto>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateReportedContent_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockService
                .Setup(s => s.UpdateReportedContentAsync(1, ValidUpdateDto))
                .ReturnsAsync(ReportedContentService<ReportedContentReadDto>
                    .BadResult("Failed to update reported content", 500,
                        new List<string> { "DB error" }));

            // Act
            var actionResult = await _controller.UpdateReportedContent(1, ValidUpdateDto);
            var result = actionResult.Result as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE /api/ReportedContents/{id}  →  IActionResult, rzutujemy wprost
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteReportedContent_Returns200_WhenSuccess()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteReportedContentAsync(1))
                .ReturnsAsync(ReportedContentService<bool>
                    .GoodResult("Reported content deleted successfully", 200, true));

            // Act
            var result = await _controller.DeleteReportedContent(1) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        [Fact]
        public async Task DeleteReportedContent_Returns404_WhenNotFound()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteReportedContentAsync(99))
                .ReturnsAsync(ReportedContentService<bool>
                    .BadResult("Reported content with ID 99 not found", 404, data: false));

            // Act
            var result = await _controller.DeleteReportedContent(99) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        [Fact]
        public async Task DeleteReportedContent_Returns500_WhenExceptionOccurs()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteReportedContentAsync(1))
                .ReturnsAsync(ReportedContentService<bool>
                    .BadResult("Failed to delete reported content", 500,
                        new List<string> { "DB error" }, false));

            // Act
            var result = await _controller.DeleteReportedContent(1) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result!.StatusCode);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Weryfikacja liczby wywołań serwisu
        // ─────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateReportedContent_CallsServiceExactlyOnce_WhenModelValid()
        {
            // Arrange
            _mockService
                .Setup(s => s.CreateReportedContentAsync(It.IsAny<CreateReportedContentDto>()))
                .ReturnsAsync(ReportedContentService<ReportedContentReadDto>
                    .GoodResult("Created", 201, SampleDto));

            // Act
            await _controller.CreateReportedContent(ValidCreateDto);

            // Assert
            _mockService.Verify(
                s => s.CreateReportedContentAsync(It.IsAny<CreateReportedContentDto>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteReportedContent_CallsServiceExactlyOnce()
        {
            // Arrange
            _mockService
                .Setup(s => s.DeleteReportedContentAsync(It.IsAny<int>()))
                .ReturnsAsync(ReportedContentService<bool>
                    .GoodResult("Deleted", 200, true));

            // Act
            await _controller.DeleteReportedContent(1);

            // Assert
            _mockService.Verify(s => s.DeleteReportedContentAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpdateReportedContent_CallsServiceExactlyOnce_WhenModelValid()
        {
            // Arrange
            _mockService
                .Setup(s => s.UpdateReportedContentAsync(It.IsAny<int>(), It.IsAny<UpdateReportedContentDto>()))
                .ReturnsAsync(ReportedContentService<ReportedContentReadDto>
                    .GoodResult("Updated", 200, SampleDto));

            // Act
            await _controller.UpdateReportedContent(1, ValidUpdateDto);

            // Assert
            _mockService.Verify(
                s => s.UpdateReportedContentAsync(1, It.IsAny<UpdateReportedContentDto>()),
                Times.Once);
        }
    }
}