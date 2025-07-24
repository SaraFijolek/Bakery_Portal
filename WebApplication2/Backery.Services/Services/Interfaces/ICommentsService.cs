using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;


namespace WebApplication2.Properties.Services.Interfaces
{
    public interface ICommentsService
    {
        Task<IEnumerable<CommentReadDto>> GetAllCommentsAsync();
        Task<CommentReadDto?> GetCommentByIdAsync(int id);
        Task<CommentReadDto> CreateCommentAsync(CommentCreateDto commentCreateDto);
        Task<bool> UpdateCommentAsync(int id, CommentUpdateDto commentUpdateDto);
        Task<bool> DeleteCommentAsync(int id);
        Task<bool> CommentExistsAsync(int id);
    }
}
