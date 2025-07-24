using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IMessagesService
    {
        Task<IEnumerable<MessageReadDto>> GetAllMessagesAsync();
        Task<MessageReadDto?> GetMessageAsync(int id);
        Task<MessageReadDto> CreateMessageAsync(MessageCreateDto messageCreateDto);
        Task<MessageReadDto> UpdateMessageAsync(int id, MessageUpdateDto messageUpdateDto);
        Task<bool> DeleteMessageAsync(int id);
        Task<bool> MessageExistsAsync(int id);
    }
}
