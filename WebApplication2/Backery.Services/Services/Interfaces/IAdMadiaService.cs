using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;

namespace WebApplication2.Properties.Services.Interfaces
{
    public interface IAdMadiaService
    {
        Task<ResultService<IEnumerable<AdMediaResponseDto>>> GetAllAdMediaAsync();
        Task<ResultService<AdMediaResponseDto>> GetAdMediaByIdAsync(int id);
        Task<ResultService<AdMediaResponseDto>> CreateAdMediaAsync(CreateAdMediaDto createAdMediaDto);
        Task<ResultService<bool>> UpdateAdMediaAsync(int id, UpdateAdMediaDto updateAdMediaDto);
        Task<ResultService<bool>> DeleteAdMediaAsync(int id);
        Task<bool> AdMediaExistsAsync(int id);
    }
}

