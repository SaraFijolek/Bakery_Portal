using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    // Generyczna klasa wyników - powinna być osobno
    public class MessageService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static MessageService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new MessageService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static MessageService<T> BadResult(
           string message,
           int statusCode,
           List<string>? errors = null,
           T? data = default)
            => new MessageService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    // Właściwa implementacja serwisu wiadomości
    public class MessagesService : IMessagesService
    {
        private readonly AppDbContext _context;

        public MessagesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MessageService<IEnumerable<MessageReadDto>>> GetAllMessagesAsync()
        {
            try
            {
                var messages = await _context.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .ToListAsync();

                var messageDtos = messages.Select(MapToMessageReadDto);

                return MessageService<IEnumerable<MessageReadDto>>.GoodResult(
                    "Messages retrieved successfully",
                    200,
                    messageDtos);
            }
            catch (Exception ex)
            {
                return MessageService<IEnumerable<MessageReadDto>>.BadResult(
                    "Failed to retrieve messages",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<MessageService<MessageReadDto>> GetMessageAsync(int id)
        {
            try
            {
                var message = await _context.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .FirstOrDefaultAsync(m => m.MessageId == id);

                if (message == null)
                {
                    return MessageService<MessageReadDto>.BadResult(
                        $"Message with ID {id} not found",
                        404);
                }

                var messageDto = MapToMessageReadDto(message);

                return MessageService<MessageReadDto>.GoodResult(
                    "Message retrieved successfully",
                    200,
                    messageDto);
            }
            catch (Exception ex)
            {
                return MessageService<MessageReadDto>.BadResult(
                    "Failed to retrieve message",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<MessageService<MessageReadDto>> CreateMessageAsync(MessageCreateDto messageCreateDto)
        {
            try
            {
                // Walidacja
                var validationResult = await ValidateMessageAsync(messageCreateDto.SenderId, messageCreateDto.ReceiverId);
                if (!validationResult.IsValid)
                {
                    return MessageService<MessageReadDto>.BadResult(
                        "Validation failed",
                        400,
                        validationResult.Errors);
                }

                var message = new Message
                {
                    SenderId = messageCreateDto.SenderId,
                    ReceiverId = messageCreateDto.ReceiverId,
                    Content = messageCreateDto.Content,
                    SentAt = DateTime.UtcNow // Używaj UTC zamiast DateTime.Now
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // Pobierz utworzoną wiadomość z nawigacją
                await _context.Entry(message)
                    .Reference(m => m.Sender)
                    .LoadAsync();
                await _context.Entry(message)
                    .Reference(m => m.Receiver)
                    .LoadAsync();

                var messageDto = MapToMessageReadDto(message);

                return MessageService<MessageReadDto>.GoodResult(
                    "Message created successfully",
                    201,
                    messageDto);
            }
            catch (Exception ex)
            {
                return MessageService<MessageReadDto>.BadResult(
                    "Failed to create message",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<MessageService<MessageReadDto>> UpdateMessageAsync(int id, MessageUpdateDto messageUpdateDto)
        {
            try
            {
                if (id != messageUpdateDto.MessageId)
                {
                    return MessageService<MessageReadDto>.BadResult(
                        "Message ID mismatch",
                        400,
                        new List<string> { "The provided ID does not match the message ID in the request body" });
                }

                var existingMessage = await _context.Messages.FindAsync(id);
                if (existingMessage == null)
                {
                    return MessageService<MessageReadDto>.BadResult(
                        $"Message with ID {id} not found",
                        404);
                }

                // Walidacja
                var validationResult = await ValidateMessageAsync(messageUpdateDto.SenderId, messageUpdateDto.ReceiverId);
                if (!validationResult.IsValid)
                {
                    return MessageService<MessageReadDto>.BadResult(
                        "Validation failed",
                        400,
                        validationResult.Errors);
                }

                // Aktualizacja właściwości
                existingMessage.SenderId = messageUpdateDto.SenderId;
                existingMessage.ReceiverId = messageUpdateDto.ReceiverId;
                existingMessage.SentAt = messageUpdateDto.SentAt;
                existingMessage.Content = messageUpdateDto.Content;

                _context.Entry(existingMessage).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Załaduj nawigację
                await _context.Entry(existingMessage)
                    .Reference(m => m.Sender)
                    .LoadAsync();
                await _context.Entry(existingMessage)
                    .Reference(m => m.Receiver)
                    .LoadAsync();

                var messageDto = MapToMessageReadDto(existingMessage);

                return MessageService<MessageReadDto>.GoodResult(
                    "Message updated successfully",
                    200,
                    messageDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MessageExistsAsync(id))
                {
                    return MessageService<MessageReadDto>.BadResult(
                        $"Message with ID {id} not found",
                        404);
                }

                return MessageService<MessageReadDto>.BadResult(
                    "Concurrency conflict occurred while updating message",
                    409);
            }
            catch (Exception ex)
            {
                return MessageService<MessageReadDto>.BadResult(
                    "Failed to update message",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<MessageService<bool>> DeleteMessageAsync(int id)
        {
            try
            {
                var message = await _context.Messages.FindAsync(id);
                if (message == null)
                {
                    return MessageService<bool>.BadResult(
                        $"Message with ID {id} not found",
                        404,
                        data: false);
                }

                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();

                return MessageService<bool>.GoodResult(
                    "Message deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return MessageService<bool>.BadResult(
                    "Failed to delete message",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<bool> MessageExistsAsync(int id)
        {
            return await _context.Messages.AnyAsync(e => e.MessageId == id);
        }

        // Prywatne metody pomocnicze
        private static MessageReadDto MapToMessageReadDto(Message message)
        {
            return new MessageReadDto
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                SentAt = message.SentAt,
                Content = message.Content,
                Sender = message.Sender != null ? new UserBasicDto
                {
                    UserId = message.Sender.Id,
                    Username = message.Sender.UserName,
                    Email = message.Sender.Email,
                } : null,
                Receiver = message.Receiver != null ? new UserBasicDto
                {
                    UserId = message.Receiver.Id,
                    Username = message.Receiver.UserName,
                    Email = message.Receiver.Email,
                } : null
            };
        }

        private async Task<ValidationResult> ValidateMessageAsync(string senderId, string receiverId)
        {
            var errors = new List<string>();

            var senderExists = await _context.Users.AnyAsync(u => u.Id == senderId);
            var receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);

            if (!senderExists)
                errors.Add($"Sender with ID {senderId} does not exist");
            if (!receiverExists)
                errors.Add($"Receiver with ID {receiverId} does not exist");
            if (senderId == receiverId)
                errors.Add("Sender and receiver cannot be the same user");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }

        private class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new();
        }
    }
}