using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IMessagesService
    {
        Task<MessageService<IEnumerable<MessageReadDto>>> GetAllMessagesAsync();
        Task<MessageService<MessageReadDto>> GetMessageAsync(int id);
        Task<MessageService<MessageReadDto>> CreateMessageAsync(MessageCreateDto messageCreateDto);
        Task<MessageService<MessageReadDto>> UpdateMessageAsync(int id, MessageUpdateDto messageUpdateDto);
        Task<MessageService<bool>> DeleteMessageAsync(int id);
        Task<bool> MessageExistsAsync(int id);
    }
}
