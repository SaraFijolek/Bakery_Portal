using Microsoft.EntityFrameworkCore;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly AppDbContext _context;

        public CommentsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommentReadDto>> GetAllCommentsAsync()
        {
            var comments = await _context.Comments
                .Include(c => c.Ad)
                .Include(c => c.User)
                .ToListAsync();

            return comments.Select(c => new CommentReadDto
            {
                CommentId = c.CommentId,
                AdId = c.AdId,
                UserId = c.UserId,
                CreatedAt = c.CreatedAt,
                Content = c.Content,
                Ad = c.Ad != null ? new AdBasicDto
                {
                    AdId = c.Ad.AdId,
                    Title = c.Ad.Title,
                    Description = c.Ad.Description
                    // Add other Ad properties as needed
                } : null,
                User = c.User != null ? new UserBasicDto
                {
                    UserId = c.User.UserId,
                    Username = c.User.Name,
                    Email = c.User.Email
                    // Add other User properties as needed
                } : null
            });
        }

        public async Task<CommentReadDto?> GetCommentByIdAsync(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Ad)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null)
                return null;

            return new CommentReadDto
            {
                CommentId = comment.CommentId,
                AdId = comment.AdId,
                UserId = comment.UserId,
                CreatedAt = comment.CreatedAt,
                Content = comment.Content,
                Ad = comment.Ad != null ? new AdBasicDto
                {
                    AdId = comment.Ad.AdId,
                    Title = comment.Ad.Title,
                    Description = comment.Ad.Description
                    // Add other Ad properties as needed
                } : null,
                User = comment.User != null ? new UserBasicDto
                {
                    UserId = comment.User.UserId,
                    Username = comment.User.Name,
                    Email = comment.User.Email
                    // Add other User properties as needed
                } : null
            };
        }

        public async Task<CommentReadDto> CreateCommentAsync(CommentCreateDto commentCreateDto)
        {
            var comment = new Comment
            {
                AdId = commentCreateDto.AdId,
                UserId = commentCreateDto.UserId,
                Content = commentCreateDto.Content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            var createdComment = await _context.Comments
                .Include(c => c.Ad)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CommentId == comment.CommentId);

            return new CommentReadDto
            {
                CommentId = createdComment!.CommentId,
                AdId = createdComment.AdId,
                UserId = createdComment.UserId,
                CreatedAt = createdComment.CreatedAt,
                Content = createdComment.Content,
                Ad = createdComment.Ad != null ? new AdBasicDto
                {
                    AdId = createdComment.Ad.AdId,
                    Title = createdComment.Ad.Title,
                    Description = createdComment.Ad.Description
                } : null,
                User = createdComment.User != null ? new UserBasicDto
                {
                    UserId = createdComment.User.UserId,
                    Username = createdComment.User.Name,
                    Email = createdComment.User.Email
                } : null
            };
        }

        public async Task<bool> UpdateCommentAsync(int id, CommentUpdateDto commentUpdateDto)
        {
            if (id != commentUpdateDto.CommentId)
            {
                return false;
            }

            var existingComment = await _context.Comments.FindAsync(id);
            if (existingComment == null)
            {
                return false;
            }

            // Update properties
            existingComment.AdId = commentUpdateDto.AdId;
            existingComment.UserId = commentUpdateDto.UserId;
            existingComment.Content = commentUpdateDto.Content;
            existingComment.CreatedAt = commentUpdateDto.CreatedAt;

            _context.Entry(existingComment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CommentExistsAsync(id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return false;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CommentExistsAsync(int id)
        {
            return await _context.Comments.AnyAsync(e => e.CommentId == id);
        }
    }
}
