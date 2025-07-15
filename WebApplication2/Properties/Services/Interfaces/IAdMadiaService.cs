using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdMadiaService
    {
        Task<IEnumerable<Admedia>> GetAllAdMediaAsync();
        Task<Admedia?> GetAdMediaByIdAsync(int id);
        Task<Admedia> CreateAdMediaAsync(Admedia adMedia);
        Task<bool> UpdateAdMediaAsync(int id, Admedia adMedia);
        Task<bool> DeleteAdMediaAsync(int id);
        Task<bool> AdMediaExistsAsync(int id);
    }
}
