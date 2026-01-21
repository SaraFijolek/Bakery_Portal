using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminsService
    {
  Task<AdminService<IEnumerable<AdminDto>>> GetAllAdminsAsync();
   Task<AdminService<AdminDto>> GetAdminByIdAsync(string id);
   Task<AdminService<AdminDto>> CreateAdminAsync(CreateAdminDto createDto);
   Task<AdminService<AdminDto>> UpdateAdminAsync(string id, UpdateAdminDto updateDto);
    Task<AdminService<bool>> DeleteAdminAsync(string id);
    Task<bool> AdminExistsAsync(string id);
    Task<AdminService<IEnumerable<AdminListDto>>> GetAllAdminsListAsync();
     Task<AdminService<AdminProfileDto>> GetAdminProfileAsync(string id);
     Task<AdminService<IEnumerable<AdminDto>>> GetActiveAdminsAsync();
     Task<AdminService<IEnumerable<AdminDto>>> GetAdminsByRoleAsync(string role);
     Task<AdminService<bool>> ChangeAdminPasswordAsync(int id, ChangeAdminPasswordDto changePasswordDto);
     Task<AdminService<bool>> Setup2FAAsync(int id, Setup2FADto setup2FADto);
     Task<AdminService<AdminDto>> GetAdminByEmailAsync(string email);
    }
}
