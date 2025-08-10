using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdminsService
    {
  Task<AdminService<IEnumerable<AdminDto>>> GetAllAdminsAsync();
   Task<AdminService<AdminDto>> GetAdminByIdAsync(int id);
   Task<AdminService<AdminDto>> CreateAdminAsync(CreateAdminDto createDto);
   Task<AdminService<AdminDto>> UpdateAdminAsync(int id, UpdateAdminDto updateDto);
    Task<AdminService<bool>> DeleteAdminAsync(int id);
    Task<bool> AdminExistsAsync(int id);
    Task<AdminService<IEnumerable<AdminListDto>>> GetAllAdminsListAsync();
     Task<AdminService<AdminProfileDto>> GetAdminProfileAsync(int id);
     Task<AdminService<IEnumerable<AdminDto>>> GetActiveAdminsAsync();
     Task<AdminService<IEnumerable<AdminDto>>> GetAdminsByRoleAsync(string role);
     Task<AdminService<bool>> ChangeAdminPasswordAsync(int id, ChangeAdminPasswordDto changePasswordDto);
     Task<AdminService<bool>> Setup2FAAsync(int id, Setup2FADto setup2FADto);
     Task<AdminService<AdminDto>> GetAdminByEmailAsync(string email);
    }
}
