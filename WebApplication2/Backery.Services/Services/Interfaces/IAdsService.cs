using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Models;
using WebApplication2.Properties.Models;


namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdsService
    {

        Task<IEnumerable<AdResponseDto>> GetAllAdsAsync();
        Task<AdResponseDto?> GetAdByIdAsync(int id);
        Task<AdResponseDto> CreateAdAsync(AdCreateDto dto);
        Task<bool> UpdateAdAsync(AdUpdateDto dto);
        Task<bool> DeleteAdAsync(int id);
        Task<bool> AdExistsAsync(int id);
    }
}
