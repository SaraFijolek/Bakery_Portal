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

        Task<AdService<IEnumerable<AdResponseDto>>> GetAllAdsAsync();

        Task<AdService<AdResponseDto>> GetAdByIdAsync(int id);
        Task<AdService<AdResponseDto>> CreateAdAsync(AdCreateDto dto);
        Task<AdService<bool>> UpdateAdAsync(AdUpdateDto dto);
        Task<AdService<bool>> DeleteAdAsync(int id);
        Task<AdService<bool>> AdExistsAsync(int id);


    }
}
