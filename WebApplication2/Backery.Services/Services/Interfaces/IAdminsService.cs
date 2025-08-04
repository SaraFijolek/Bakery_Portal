using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminsService
    {
        Task<IEnumerable<AdminDto>> GetAllAdminsAsync();
  Task<AdminDto?> GetAdminByIdAsync(int id);
  Task<AdminDto> CreateAdminAsync(CreateAdminDto createDto);
  Task<bool> UpdateAdminAsync(int id, UpdateAdminDto updateDto);
  Task<bool> DeleteAdminAsync(int id);
  Task<bool> AdminExistsAsync(int id);

  // Additional methods for different use cases
  Task<IEnumerable<AdminListDto>> GetAllAdminsListAsync();
  Task<AdminProfileDto?> GetAdminProfileAsync(int id);
  Task<IEnumerable<AdminDto>> GetActiveAdminsAsync();
  Task<IEnumerable<AdminDto>> GetAdminsByRoleAsync(string role);
  Task<bool> ChangeAdminPasswordAsync(int id, ChangeAdminPasswordDto changePasswordDto);
  Task<bool> Setup2FAAsync(int id, Setup2FADto setup2FADto);
  Task<AdminDto?> GetAdminByEmailAsync(string email);
    }
}
