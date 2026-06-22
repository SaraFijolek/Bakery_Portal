using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class CommentsControllerTests
    {
        private readonly Mock<ICommentsService> _mockCommentsService;
        private readonly CommentsController _controller;

        // Shared test data
        private readonly CommentReadDto _sampleComment;
        private readonly List<CommentReadDto> _sampleComments;
        private readonly CommentCreateDto _sampleCreateDto;
        private readonly CommentUpdateDto _sampleUpdateDto;

        public CommentsControllerTests()
        {
            _mockCommentsService = new Mock<ICommentsService>();
            _controller = new CommentsController(_mockCommentsService.Object);

            _sampleComment = new CommentReadDto
            {
                CommentId = 1,
                AdId = 1,
                UserId = "user-123",
                Content = "Test comment",
                CreatedAt = DateTime.Now,
                Ad = new AdBasicDto { AdId = 1, Title = "Test Ad", Description = "Test Description" },
                User = new UserBasicDto { UserId = "user-123", Username = "TestUser", Email = "test@test.com" }
            };

            _sampleComments = new List<CommentReadDto> { _sampleComment };

            _sampleCreateDto = new CommentCreateDto
            {
                AdId = 1,
                UserId = "user-123",
                Content = "New comment"
            };

            _sampleUpdateDto = new CommentUpdateDto
            {
                CommentId = 1,
                AdId = 1,
                UserId = "user-123",
                Content = "Updated comment",
                CreatedAt = DateTime.Now
            };
        }

        #region GetComments

        [Fact]
        public async Task GetComments_Returns200_WithListOfComments()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.GetAllCommentsAsync())
                .ReturnsAsync(CommentService<IEnumerable<CommentReadDto>>.GoodResult(
                    "Comments retrieved successfully", 200, _sampleComments));

            // Act
            var result = await _controller.GetComments();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusResult.StatusCode);
            var returnedData = Assert.IsAssignableFrom<IEnumerable<CommentReadDto>>(statusResult.Value);
            Assert.Single(returnedData);
        }

        [Fact]
        public async Task GetComments_Returns200_WhenListIsEmpty()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.GetAllCommentsAsync())
                .ReturnsAsync(CommentService<IEnumerable<CommentReadDto>>.GoodResult(
                    "Comments retrieved successfully", 200, new List<CommentReadDto>()));

            // Act
            var result = await _controller.GetComments();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetComments_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.GetAllCommentsAsync())
                .ReturnsAsync(CommentService<IEnumerable<CommentReadDto>>.BadResult(
                    "Failed to retrieve comments", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.GetComments();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetComments_ReturnsErrorObject_WhenServiceFails()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.GetAllCommentsAsync())
                .ReturnsAsync(CommentService<IEnumerable<CommentReadDto>>.BadResult(
                    "Failed to retrieve comments", 500,
                    new List<string> { "Database connection lost" }));

            // Act
            var result = await _controller.GetComments();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            dynamic value = statusResult.Value!;
            Assert.NotNull(value);
        }

        #endregion

        #region GetComment

        [Fact]
        public async Task GetComment_Returns200_WhenCommentExists()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.GetCommentByIdAsync(1))
                .ReturnsAsync(CommentService<CommentReadDto>.GoodResult(
                    "Comment retrieved successfully", 200, _sampleComment));

            // Act
            var result = await _controller.GetComment(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusResult.StatusCode);
            var returnedComment = Assert.IsType<CommentReadDto>(statusResult.Value);
            Assert.Equal(1, returnedComment.CommentId);
            Assert.Equal("Test comment", returnedComment.Content);
        }

        [Fact]
        public async Task GetComment_Returns404_WhenCommentNotFound()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.GetCommentByIdAsync(99))
                .ReturnsAsync(CommentService<CommentReadDto>.BadResult(
                    "Comment with ID 99 not found", 404));

            // Act
            var result = await _controller.GetComment(99);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetComment_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.GetCommentByIdAsync(1))
                .ReturnsAsync(CommentService<CommentReadDto>.BadResult(
                    "Failed to retrieve comment", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.GetComment(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetComment_ReturnsCommentWithNavigationProps_WhenExists()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.GetCommentByIdAsync(1))
                .ReturnsAsync(CommentService<CommentReadDto>.GoodResult(
                    "Comment retrieved successfully", 200, _sampleComment));

            // Act
            var result = await _controller.GetComment(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            var returnedComment = Assert.IsType<CommentReadDto>(statusResult.Value);
            Assert.NotNull(returnedComment.Ad);
            Assert.NotNull(returnedComment.User);
            Assert.Equal("TestUser", returnedComment.User!.Username);
            Assert.Equal("Test Ad", returnedComment.Ad!.Title);
        }

        #endregion

        #region CreateComment

        [Fact]
        public async Task CreateComment_Returns201_WhenCreatedSuccessfully()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.CreateCommentAsync(_sampleCreateDto))
                .ReturnsAsync(CommentService<CommentReadDto>.GoodResult(
                    "Comment created successfully", 201, _sampleComment));

            // Act
            var result = await _controller.CreateComment(_sampleCreateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(201, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateComment_Returns400_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("AdId", "AdId is required");

            // Act
            var result = await _controller.CreateComment(new CommentCreateDto());

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateComment_Returns400_WhenValidationFailsInService()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.CreateCommentAsync(It.IsAny<CommentCreateDto>()))
                .ReturnsAsync(CommentService<CommentReadDto>.BadResult(
                    "Validation failed", 400,
                    new List<string> { "Ad with ID 999 does not exist" }));

            // Act
            var result = await _controller.CreateComment(_sampleCreateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateComment_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.CreateCommentAsync(It.IsAny<CommentCreateDto>()))
                .ReturnsAsync(CommentService<CommentReadDto>.BadResult(
                    "Failed to create comment", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.CreateComment(_sampleCreateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateComment_DoesNotCallService_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserId", "UserId is required");

            // Act
            await _controller.CreateComment(new CommentCreateDto());

            // Assert
            _mockCommentsService.Verify(
                s => s.CreateCommentAsync(It.IsAny<CommentCreateDto>()),
                Times.Never);
        }

        #endregion

        #region UpdateComment

        [Fact]
        public async Task UpdateComment_Returns200_WhenUpdatedSuccessfully()
        {
            // Arrange
            var updatedComment = new CommentReadDto
            {
                CommentId = 1,
                AdId = 1,
                UserId = "user-123",
                Content = "Updated comment",
                CreatedAt = DateTime.Now
            };

            _mockCommentsService
                .Setup(s => s.UpdateCommentAsync(1, _sampleUpdateDto))
                .ReturnsAsync(CommentService<CommentReadDto>.GoodResult(
                    "Comment updated successfully", 200, updatedComment));

            // Act
            var result = await _controller.UpdateComment(1, _sampleUpdateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateComment_Returns400_WhenIdMismatch()
        {
            // Arrange
            var mismatchedDto = new CommentUpdateDto
            {
                CommentId = 99, // Different from URL id = 1
                AdId = 1,
                UserId = "user-123",
                Content = "Content",
                CreatedAt = DateTime.Now
            };

            // Act
            var result = await _controller.UpdateComment(1, mismatchedDto);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateComment_Returns400_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Content", "Content is required");

            // Act
            var result = await _controller.UpdateComment(1, _sampleUpdateDto);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateComment_Returns404_WhenCommentNotFound()
        {
            // Arrange
            var updateDto = new CommentUpdateDto
            {
                CommentId = 99,
                AdId = 1,
                UserId = "user-123",
                Content = "Content",
                CreatedAt = DateTime.Now
            };

            _mockCommentsService
                .Setup(s => s.UpdateCommentAsync(99, updateDto))
                .ReturnsAsync(CommentService<CommentReadDto>.BadResult(
                    "Comment with ID 99 not found", 404));

            // Act
            var result = await _controller.UpdateComment(99, updateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateComment_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.UpdateCommentAsync(1, _sampleUpdateDto))
                .ReturnsAsync(CommentService<CommentReadDto>.BadResult(
                    "Failed to update comment", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.UpdateComment(1, _sampleUpdateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateComment_DoesNotCallService_WhenIdMismatch()
        {
            // Arrange
            var mismatchedDto = new CommentUpdateDto { CommentId = 99 };

            // Act
            await _controller.UpdateComment(1, mismatchedDto);

            // Assert
            _mockCommentsService.Verify(
                s => s.UpdateCommentAsync(It.IsAny<int>(), It.IsAny<CommentUpdateDto>()),
                Times.Never);
        }

        #endregion

        #region DeleteComment

        [Fact]
        public async Task DeleteComment_Returns200_WhenDeletedSuccessfully()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.DeleteCommentAsync(1))
                .ReturnsAsync(CommentService<bool>.GoodResult(
                    "Comment deleted successfully", 200, true));

            // Act
            var result = await _controller.DeleteComment(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, statusResult.StatusCode);
            Assert.Equal(true, statusResult.Value);
        }

        [Fact]
        public async Task DeleteComment_Returns404_WhenCommentNotFound()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.DeleteCommentAsync(99))
                .ReturnsAsync(CommentService<bool>.BadResult(
                    "Comment with ID 99 not found", 404, null, false));

            // Act
            var result = await _controller.DeleteComment(99);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteComment_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.DeleteCommentAsync(1))
                .ReturnsAsync(CommentService<bool>.BadResult(
                    "Failed to delete comment", 500,
                    new List<string> { "Database error" }, false));

            // Act
            var result = await _controller.DeleteComment(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteComment_CallsServiceOnce_WithCorrectId()
        {
            // Arrange
            _mockCommentsService
                .Setup(s => s.DeleteCommentAsync(1))
                .ReturnsAsync(CommentService<bool>.GoodResult(
                    "Comment deleted successfully", 200, true));

            // Act
            await _controller.DeleteComment(1);

            // Assert
            _mockCommentsService.Verify(
                s => s.DeleteCommentAsync(1),
                Times.Once);
        }

        #endregion
    }
}