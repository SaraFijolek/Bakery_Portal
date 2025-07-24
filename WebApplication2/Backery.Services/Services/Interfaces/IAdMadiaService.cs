using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdMadiaService
    {
        Task<IEnumerable<AdMediaResponseDto>> GetAllAdMediaAsync();
        Task<AdMediaResponseDto?> GetAdMediaByIdAsync(int id);
        Task<AdMediaResponseDto> CreateAdMediaAsync(CreateAdMediaDto createAdMediaDto);
        Task<bool> UpdateAdMediaAsync(int id, UpdateAdMediaDto updateAdMediaDto);
        Task<bool> DeleteAdMediaAsync(int id);
        Task<bool> AdMediaExistsAsync(int id);
    }
}

