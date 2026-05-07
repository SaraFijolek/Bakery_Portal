using WebApplication2.DTO;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using static WebApplication2.Properties.Services.AdminsSessionsService;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminsSessionsService
    {
        Task<AdminSessionService<List<AdminSessionReadDto>>> GetAllAdminSessionsAsync();
        Task<AdminSessionService<List<AdminSessionListDto>>> GetAllAdminSessionsListAsync();
        Task<AdminSessionService<AdminSessionReadDto>> GetAdminSessionByIdAsync(int id);
        Task<AdminSessionService<AdminSessionReadDto>> CreateAdminSessionAsync(CreateAdminSessionDto sessionDto);
        Task<AdminSessionService<bool>> UpdateAdminSessionAsync(int id, UpdateAdminSessionDto sessionDto);
        Task<AdminSessionService<bool>> DeleteAdminSessionAsync(int id);
        Task<bool> AdminSessionExistsAsync(int id);
    }
}
