using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication2.Controllers;
using WebApplication2.DTO;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;
using Xunit;

namespace WebApplication2.Tests.Controllers
{
    public class MessagesControllerTests
    {
        private readonly Mock<IMessagesService> _mockMessagesService;
        private readonly MessagesController _controller;

        // Shared test data
        private readonly MessageReadDto _sampleMessage;
        private readonly List<MessageReadDto> _sampleMessages;
        private readonly MessageCreateDto _sampleCreateDto;
        private readonly MessageUpdateDto _sampleUpdateDto;

        public MessagesControllerTests()
        {
            _mockMessagesService = new Mock<IMessagesService>();
            _controller = new MessagesController(_mockMessagesService.Object);

            _sampleMessage = new MessageReadDto
            {
                MessageId = 1,
                SenderId = "sender-123",
                ReceiverId = "receiver-456",
                Content = "Test message",
                SentAt = DateTime.UtcNow,
                Sender = new UserBasicDto
                {
                    UserId = "sender-123",
                    Username = "SenderUser",
                    Email = "sender@test.com"
                },
                Receiver = new UserBasicDto
                {
                    UserId = "receiver-456",
                    Username = "ReceiverUser",
                    Email = "receiver@test.com"
                }
            };

            _sampleMessages = new List<MessageReadDto> { _sampleMessage };

            _sampleCreateDto = new MessageCreateDto
            {
                SenderId = "sender-123",
                ReceiverId = "receiver-456",
                Content = "New message"
            };

            _sampleUpdateDto = new MessageUpdateDto
            {
                MessageId = 1,
                SenderId = "sender-123",
                ReceiverId = "receiver-456",
                Content = "Updated message",
                SentAt = DateTime.UtcNow
            };
        }

        #region GetMessages

        [Fact]
        public async Task GetMessages_Returns200_WithListOfMessages()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.GetAllMessagesAsync())
                .ReturnsAsync(MessageService<IEnumerable<MessageReadDto>>.GoodResult(
                    "Messages retrieved successfully", 200, _sampleMessages));

            // Act
            var result = await _controller.GetMessages();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusResult.StatusCode);
            var returnedData = Assert.IsAssignableFrom<IEnumerable<MessageReadDto>>(statusResult.Value);
            Assert.Single(returnedData);
        }

        [Fact]
        public async Task GetMessages_Returns200_WhenListIsEmpty()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.GetAllMessagesAsync())
                .ReturnsAsync(MessageService<IEnumerable<MessageReadDto>>.GoodResult(
                    "Messages retrieved successfully", 200, new List<MessageReadDto>()));

            // Act
            var result = await _controller.GetMessages();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusResult.StatusCode);
            var returnedData = Assert.IsAssignableFrom<IEnumerable<MessageReadDto>>(statusResult.Value);
            Assert.Empty(returnedData);
        }

        [Fact]
        public async Task GetMessages_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.GetAllMessagesAsync())
                .ReturnsAsync(MessageService<IEnumerable<MessageReadDto>>.BadResult(
                    "Failed to retrieve messages", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.GetMessages();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetMessages_ReturnsMessagesWithSenderAndReceiver()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.GetAllMessagesAsync())
                .ReturnsAsync(MessageService<IEnumerable<MessageReadDto>>.GoodResult(
                    "Messages retrieved successfully", 200, _sampleMessages));

            // Act
            var result = await _controller.GetMessages();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            var returnedData = Assert.IsAssignableFrom<IEnumerable<MessageReadDto>>(statusResult.Value);
            var message = returnedData.First();
            Assert.NotNull(message.Sender);
            Assert.NotNull(message.Receiver);
            Assert.Equal("SenderUser", message.Sender!.Username);
            Assert.Equal("ReceiverUser", message.Receiver!.Username);
        }

        #endregion

        #region GetMessage

        [Fact]
        public async Task GetMessage_Returns200_WhenMessageExists()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.GetMessageAsync(1))
                .ReturnsAsync(MessageService<MessageReadDto>.GoodResult(
                    "Message retrieved successfully", 200, _sampleMessage));

            // Act
            var result = await _controller.GetMessage(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, statusResult.StatusCode);
            var returnedMessage = Assert.IsType<MessageReadDto>(statusResult.Value);
            Assert.Equal(1, returnedMessage.MessageId);
            Assert.Equal("Test message", returnedMessage.Content);
        }

        [Fact]
        public async Task GetMessage_Returns404_WhenMessageNotFound()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.GetMessageAsync(99))
                .ReturnsAsync(MessageService<MessageReadDto>.BadResult(
                    "Message with ID 99 not found", 404));

            // Act
            var result = await _controller.GetMessage(99);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetMessage_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.GetMessageAsync(1))
                .ReturnsAsync(MessageService<MessageReadDto>.BadResult(
                    "Failed to retrieve message", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.GetMessage(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetMessage_ReturnsMessageWithNavProps_WhenExists()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.GetMessageAsync(1))
                .ReturnsAsync(MessageService<MessageReadDto>.GoodResult(
                    "Message retrieved successfully", 200, _sampleMessage));

            // Act
            var result = await _controller.GetMessage(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            var returnedMessage = Assert.IsType<MessageReadDto>(statusResult.Value);
            Assert.NotNull(returnedMessage.Sender);
            Assert.NotNull(returnedMessage.Receiver);
            Assert.Equal("sender-123", returnedMessage.SenderId);
            Assert.Equal("receiver-456", returnedMessage.ReceiverId);
        }

        #endregion

        #region CreateMessage

        [Fact]
        public async Task CreateMessage_Returns201_WhenCreatedSuccessfully()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.CreateMessageAsync(_sampleCreateDto))
                .ReturnsAsync(MessageService<MessageReadDto>.GoodResult(
                    "Message created successfully", 201, _sampleMessage));

            // Act
            var result = await _controller.CreateMessage(_sampleCreateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(201, statusResult.StatusCode);
            var returnedMessage = Assert.IsType<MessageReadDto>(statusResult.Value);
            Assert.Equal("Test message", returnedMessage.Content);
        }

        [Fact]
        public async Task CreateMessage_Returns400_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("SenderId", "SenderId is required");

            // Act
            var result = await _controller.CreateMessage(new MessageCreateDto());

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateMessage_Returns400_WhenSenderDoesNotExist()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.CreateMessageAsync(It.IsAny<MessageCreateDto>()))
                .ReturnsAsync(MessageService<MessageReadDto>.BadResult(
                    "Validation failed", 400,
                    new List<string> { "Sender with ID invalid-id does not exist" }));

            // Act
            var result = await _controller.CreateMessage(_sampleCreateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateMessage_Returns400_WhenSenderAndReceiverAreSame()
        {
            // Arrange
            var sameUserDto = new MessageCreateDto
            {
                SenderId = "user-123",
                ReceiverId = "user-123",
                Content = "Message to myself"
            };

            _mockMessagesService
                .Setup(s => s.CreateMessageAsync(It.IsAny<MessageCreateDto>()))
                .ReturnsAsync(MessageService<MessageReadDto>.BadResult(
                    "Validation failed", 400,
                    new List<string> { "Sender and receiver cannot be the same user" }));

            // Act
            var result = await _controller.CreateMessage(sameUserDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateMessage_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.CreateMessageAsync(It.IsAny<MessageCreateDto>()))
                .ReturnsAsync(MessageService<MessageReadDto>.BadResult(
                    "Failed to create message", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.CreateMessage(_sampleCreateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateMessage_DoesNotCallService_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("ReceiverId", "ReceiverId is required");

            // Act
            await _controller.CreateMessage(new MessageCreateDto());

            // Assert
            _mockMessagesService.Verify(
                s => s.CreateMessageAsync(It.IsAny<MessageCreateDto>()),
                Times.Never);
        }

        #endregion

        #region UpdateMessage

        [Fact]
        public async Task UpdateMessage_Returns200_WhenUpdatedSuccessfully()
        {
            // Arrange
            var updatedMessage = new MessageReadDto
            {
                MessageId = 1,
                SenderId = "sender-123",
                ReceiverId = "receiver-456",
                Content = "Updated message",
                SentAt = DateTime.UtcNow
            };

            _mockMessagesService
                .Setup(s => s.UpdateMessageAsync(1, _sampleUpdateDto))
                .ReturnsAsync(MessageService<MessageReadDto>.GoodResult(
                    "Message updated successfully", 200, updatedMessage));

            // Act
            var result = await _controller.UpdateMessage(1, _sampleUpdateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, statusResult.StatusCode);
            var returnedMessage = Assert.IsType<MessageReadDto>(statusResult.Value);
            Assert.Equal("Updated message", returnedMessage.Content);
        }

        [Fact]
        public async Task UpdateMessage_Returns400_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("SenderId", "SenderId is required");

            // Act
            var result = await _controller.UpdateMessage(1, new MessageUpdateDto());

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateMessage_Returns400_WhenIdMismatchInService()
        {
            // Arrange
            var mismatchedDto = new MessageUpdateDto
            {
                MessageId = 99,
                SenderId = "sender-123",
                ReceiverId = "receiver-456",
                Content = "Content",
                SentAt = DateTime.UtcNow
            };

            _mockMessagesService
                .Setup(s => s.UpdateMessageAsync(1, mismatchedDto))
                .ReturnsAsync(MessageService<MessageReadDto>.BadResult(
                    "Message ID mismatch", 400,
                    new List<string> { "The provided ID does not match the message ID in the request body" }));

            // Act
            var result = await _controller.UpdateMessage(1, mismatchedDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateMessage_Returns404_WhenMessageNotFound()
        {
            // Arrange
            var updateDto = new MessageUpdateDto
            {
                MessageId = 99,
                SenderId = "sender-123",
                ReceiverId = "receiver-456",
                Content = "Content",
                SentAt = DateTime.UtcNow
            };

            _mockMessagesService
                .Setup(s => s.UpdateMessageAsync(99, updateDto))
                .ReturnsAsync(MessageService<MessageReadDto>.BadResult(
                    "Message with ID 99 not found", 404));

            // Act
            var result = await _controller.UpdateMessage(99, updateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateMessage_Returns409_WhenConcurrencyConflict()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.UpdateMessageAsync(1, _sampleUpdateDto))
                .ReturnsAsync(MessageService<MessageReadDto>.BadResult(
                    "Concurrency conflict occurred while updating message", 409));

            // Act
            var result = await _controller.UpdateMessage(1, _sampleUpdateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateMessage_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.UpdateMessageAsync(1, _sampleUpdateDto))
                .ReturnsAsync(MessageService<MessageReadDto>.BadResult(
                    "Failed to update message", 500,
                    new List<string> { "Database error" }));

            // Act
            var result = await _controller.UpdateMessage(1, _sampleUpdateDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateMessage_DoesNotCallService_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("SenderId", "SenderId is required");

            // Act
            await _controller.UpdateMessage(1, new MessageUpdateDto());

            // Assert
            _mockMessagesService.Verify(
                s => s.UpdateMessageAsync(It.IsAny<int>(), It.IsAny<MessageUpdateDto>()),
                Times.Never);
        }

        #endregion

        #region DeleteMessage

        [Fact]
        public async Task DeleteMessage_Returns200_WhenDeletedSuccessfully()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.DeleteMessageAsync(1))
                .ReturnsAsync(MessageService<bool>.GoodResult(
                    "Message deleted successfully", 200, true));

            // Act
            var result = await _controller.DeleteMessage(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteMessage_Returns404_WhenMessageNotFound()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.DeleteMessageAsync(99))
                .ReturnsAsync(MessageService<bool>.BadResult(
                    "Message with ID 99 not found", 404, null, false));

            // Act
            var result = await _controller.DeleteMessage(99);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteMessage_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.DeleteMessageAsync(1))
                .ReturnsAsync(MessageService<bool>.BadResult(
                    "Failed to delete message", 500,
                    new List<string> { "Database error" }, false));

            // Act
            var result = await _controller.DeleteMessage(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteMessage_CallsServiceOnce_WithCorrectId()
        {
            // Arrange
            _mockMessagesService
                .Setup(s => s.DeleteMessageAsync(1))
                .ReturnsAsync(MessageService<bool>.GoodResult(
                    "Message deleted successfully", 200, true));

            // Act
            await _controller.DeleteMessage(1);

            // Assert
            _mockMessagesService.Verify(
                s => s.DeleteMessageAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_AlwaysReturnsResponseBody()
        {
            // Arrange
            // Uwaga: W obecnej implementacji kontrolera DeleteMessage zawsze
            // zwraca success = false w body, niezależnie od wyniku.
            // Ten test dokumentuje tę lukę w implementacji.
            _mockMessagesService
                .Setup(s => s.DeleteMessageAsync(1))
                .ReturnsAsync(MessageService<bool>.GoodResult(
                    "Message deleted successfully", 200, true));

            // Act
            var result = await _controller.DeleteMessage(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.NotNull(statusResult.Value);
        }

        #endregion
    }
}