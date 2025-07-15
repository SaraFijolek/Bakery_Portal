using WebApplication2.Properties.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IMessagesService
    {
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Task<Message?>GetMessageAsync(int id);
        Task<Message>CreateMessageAsync(Message message);
        Task<Message>UpdateMessageAsync(int id,Message message);
        Task<bool>DeleteMessageAsync(int id);
        Task<bool>MessageExistsAsync(int id);
    }
}
