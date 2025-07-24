using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{

    public class MessagesService : IMessagesService
    {
        private readonly AppDbContext _context;

        public MessagesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MessageReadDto>> GetAllMessagesAsync()
        {
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();

            return messages.Select(m => new MessageReadDto
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                SentAt = m.SentAt,
                Content = m.Content,
                Sender = m.Sender != null ? new UserBasicDto
                {
                    UserId = m.Sender.UserId,
                    Username = m.Sender.Name,
                    Email = m.Sender.Email,
                    
                } : null,
                Receiver = m.Receiver != null ? new UserBasicDto
                {
                    UserId = m.Receiver.UserId,
                    Username = m.Receiver.Name,
                    Email = m.Receiver.Email,
                    
                } : null
            });
        }

        public async Task<MessageReadDto?> GetMessageAsync(int id)
        {
            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.MessageId == id);

            if (message == null)
                return null;

            return new MessageReadDto
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                SentAt = message.SentAt,
                Content = message.Content,
                Sender = message.Sender != null ? new UserBasicDto
                {
                    UserId = message.Sender.UserId,
                    Username = message.Sender.Name,
                    Email = message.Sender.Email,
                   
                    // Add other User properties as needed
                } : null,
                Receiver = message.Receiver != null ? new UserBasicDto
                {
                    UserId = message.Receiver.UserId,
                    Username = message.Receiver.Name,
                    Email = message.Receiver.Email,
                   
                } : null
            };
        }

        public async Task<MessageReadDto> CreateMessageAsync(MessageCreateDto messageCreateDto)
        {
            var message = new Message
            {
                SenderId = messageCreateDto.SenderId,
                ReceiverId = messageCreateDto.ReceiverId,
                Content = messageCreateDto.Content,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

           
            var createdMessage = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.MessageId == message.MessageId);

            return new MessageReadDto
            {
                MessageId = createdMessage!.MessageId,
                SenderId = createdMessage.SenderId,
                ReceiverId = createdMessage.ReceiverId,
                SentAt = createdMessage.SentAt,
                Content = createdMessage.Content,
                Sender = createdMessage.Sender != null ? new UserBasicDto
                {
                    UserId = createdMessage.Sender.UserId,
                    Username = createdMessage.Sender.Name,
                    Email = createdMessage.Sender.Email,
                    
                } : null,
                Receiver = createdMessage.Receiver != null ? new UserBasicDto
                {
                    UserId = createdMessage.Receiver.UserId,
                    Username = createdMessage.Receiver.Name,
                    Email = createdMessage.Receiver.Email,
                    
                } : null
            };
        }

        public async Task<MessageReadDto> UpdateMessageAsync(int id, MessageUpdateDto messageUpdateDto)
        {
            if (id != messageUpdateDto.MessageId)
                throw new ArgumentException("Message ID mismatch");

            var existingMessage = await _context.Messages.FindAsync(id);
            if (existingMessage == null)
                throw new KeyNotFoundException($"Message with ID {id} not found");

            // Update properties
            existingMessage.SenderId = messageUpdateDto.SenderId;
            existingMessage.ReceiverId = messageUpdateDto.ReceiverId;
            existingMessage.SentAt = messageUpdateDto.SentAt;
            existingMessage.Content = messageUpdateDto.Content;

            _context.Entry(existingMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MessageExistsAsync(id))
                    throw new KeyNotFoundException($"Message with ID {id} not found");
                throw;
            }

            // Reload with navigation properties
            var updatedMessage = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.MessageId == id);

            return new MessageReadDto
            {
                MessageId = updatedMessage!.MessageId,
                SenderId = updatedMessage.SenderId,
                ReceiverId = updatedMessage.ReceiverId,
                SentAt = updatedMessage.SentAt,
                Content = updatedMessage.Content,
                Sender = updatedMessage.Sender != null ? new UserBasicDto
                {
                    UserId = updatedMessage.Sender.UserId,
                    Username = updatedMessage.Sender.Name,
                    Email = updatedMessage.Sender.Email,
                    
                } : null,
                Receiver = updatedMessage.Receiver != null ? new UserBasicDto
                {
                    UserId = updatedMessage.Receiver.UserId,
                    Username = updatedMessage.Receiver.Name,
                    Email = updatedMessage.Receiver.Email,
                    
                } : null
            };
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
                return false;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MessageExistsAsync(int id)
        {
            return await _context.Messages.AnyAsync(e => e.MessageId == id);
        }
    }
}