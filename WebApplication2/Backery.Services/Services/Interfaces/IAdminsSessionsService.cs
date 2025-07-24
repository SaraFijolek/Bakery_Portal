using WebApplication2.DTO;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminsSessionsService
    {
        Task<List<AdminSessionReadDto>> GetAllAdminSessionsAsync();
        Task<List<AdminSessionListDto>> GetAllAdminSessionsListAsync();
        Task<AdminSessionReadDto> GetAdminSessionByIdAsync(int id);
        Task<AdminSessionReadDto> CreateAdminSessionAsync(CreateAdminSessionDto sessionDto);
        Task<bool> UpdateAdminSessionAsync(int id, UpdateAdminSessionDto sessionDto);
        Task<bool> DeleteAdminSessionAsync(int id);
        Task<bool> AdminSessionExistsAsync(int id);
    }
}
