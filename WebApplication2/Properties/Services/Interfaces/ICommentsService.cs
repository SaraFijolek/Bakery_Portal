using WebApplication2.Properties.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace WebApplication2.Properties.Services.Interfaces
{
    public interface ICommentsService
    {
        Task<IEnumerable<Comment>> GetAllCommentsAsync();    
        Task<Comment?> GetCommentByIdAsync(int id);          
        Task<Comment> CreateCommentAsync(Comment comment);   
        Task<bool> UpdateCommentAsync(int id, Comment comment); 
        Task<bool> DeleteCommentAsync(int id);               
        Task<bool> CommentExistsAsync(int id);
    }
}
