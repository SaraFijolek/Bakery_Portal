using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApplication2.Backery.API.DTO
{
    public class UploadAdMediaDto
    {
        [Required]
        public int AdId { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
