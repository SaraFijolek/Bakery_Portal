using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();
        }

        public async Task<Message?> GetMessageAsync(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.MessageId == id);
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message> UpdateMessageAsync(int id, Message message)
        {
            if (id != message.MessageId)
                throw new ArgumentException("Message ID mismatch");

            _context.Entry(message).State = EntityState.Modified;

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

            return message;
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