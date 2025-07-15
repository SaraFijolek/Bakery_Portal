using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Properties.Models;


namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdsService
    {
        Task<IEnumerable<Ad>> GetAllAdsAsync();
        Task<Ad?> GetAdByIdAsync(int id);
        Task<Ad> CreateAdAsync(Ad ad);
        Task<bool> UpdateAdAsync(int id, Ad ad);
        Task<bool> DeleteAdAsync(int id);
        Task<bool> AdExistsAsync(int id);
    }
}
