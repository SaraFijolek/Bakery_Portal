using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Models;


namespace WebApplication2.Properties.Services.Interfaces
{
    public interface ICommentsService
    {
        Task<CommentService<IEnumerable<CommentReadDto>>> GetAllCommentsAsync();
        Task<CommentService<CommentReadDto>> GetCommentByIdAsync(int id);
        Task<CommentService<CommentReadDto>> CreateCommentAsync(CommentCreateDto commentCreateDto);
        Task<CommentService<CommentReadDto>> UpdateCommentAsync(int id, CommentUpdateDto commentUpdateDto);
        Task<CommentService<bool>> DeleteCommentAsync(int id);
        Task<bool> CommentExistsAsync(int id);
    }
}
