using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Properties.Services
{
    public class CommentService<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static CommentService<T> GoodResult(
            string message,
            int statusCode,
            T? data = default)
            => new CommentService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = true,
                Data = data
            };

        public static CommentService<T> BadResult(
            string message,
            int statusCode,
            List<string>? errors = null,
            T? data = default)
            => new CommentService<T>
            {
                Message = message,
                StatusCode = statusCode,
                Success = false,
                Errors = errors ?? new List<string>(),
                Data = data
            };
    }

    public class CommentsService : ICommentsService
    {
        private readonly AppDbContext _context;

        public CommentsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CommentService<IEnumerable<CommentReadDto>>> GetAllCommentsAsync()
        {
            try
            {
                var comments = await _context.Comments
                    .Include(c => c.Ad)
                    .Include(c => c.User)
                    .ToListAsync();

                var commentDtos = comments.Select(c => new CommentReadDto
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
                        UserId = c.UserId,
                        Username = c.User.UserName,
                        Email = c.User.Email
                        // Add other User properties as needed
                    } : null
                });

                return CommentService<IEnumerable<CommentReadDto>>.GoodResult(
                    "Comments retrieved successfully",
                    200,
                    commentDtos);
            }
            catch (Exception ex)
            {
                return CommentService<IEnumerable<CommentReadDto>>.BadResult(
                    "Failed to retrieve comments",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<CommentService<CommentReadDto>> GetCommentByIdAsync(int id)
        {
            try
            {
                var comment = await _context.Comments
                    .Include(c => c.Ad)
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CommentId == id);

                if (comment == null)
                {
                    return CommentService<CommentReadDto>.BadResult(
                        $"Comment with ID {id} not found",
                        404);
                }

                var commentDto = new CommentReadDto
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
                        UserId = comment.UserId,
                        Username = comment.User.UserName,
                        Email = comment.User.Email
                        // Add other User properties as needed
                    } : null
                };

                return CommentService<CommentReadDto>.GoodResult(
                    "Comment retrieved successfully",
                    200,
                    commentDto);
            }
            catch (Exception ex)
            {
                return CommentService<CommentReadDto>.BadResult(
                    "Failed to retrieve comment",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<CommentService<CommentReadDto>> CreateCommentAsync(CommentCreateDto commentCreateDto)
        {
            try
            {
                // Validate that AdId and UserId exist
                var adExists = await _context.Ads.AnyAsync(a => a.AdId == commentCreateDto.AdId);
                var userExists = await _context.Users.AnyAsync(u => u.Id == commentCreateDto.UserId);

                var errors = new List<string>();
                if (!adExists)
                    errors.Add($"Ad with ID {commentCreateDto.AdId} does not exist");
                if (!userExists)
                    errors.Add($"User with ID {commentCreateDto.UserId} does not exist");
                if (string.IsNullOrWhiteSpace(commentCreateDto.Content))
                    errors.Add("Comment content cannot be empty");

                if (errors.Any())
                {
                    return CommentService<CommentReadDto>.BadResult(
                        "Validation failed",
                        400,
                        errors);
                }

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

                var commentDto = new CommentReadDto
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
                        UserId = createdComment.UserId,
                        Username = createdComment.User.UserName,
                        Email = createdComment.User.Email
                    } : null
                };

                return CommentService<CommentReadDto>.GoodResult(
                    "Comment created successfully",
                    201,
                    commentDto);
            }
            catch (Exception ex)
            {
                return CommentService<CommentReadDto>.BadResult(
                    "Failed to create comment",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<CommentService<CommentReadDto>> UpdateCommentAsync(int id, CommentUpdateDto commentUpdateDto)
        {
            try
            {
                if (id != commentUpdateDto.CommentId)
                {
                    return CommentService<CommentReadDto>.BadResult(
                        "Comment ID in URL does not match Comment ID in request body",
                        400);
                }

                var existingComment = await _context.Comments.FindAsync(id);
                if (existingComment == null)
                {
                    return CommentService<CommentReadDto>.BadResult(
                        $"Comment with ID {id} not found",
                        404);
                }

                // Validate that AdId and UserId exist
                var adExists = await _context.Ads.AnyAsync(a => a.AdId == commentUpdateDto.AdId);
                var userExists = await _context.Users.AnyAsync(u => u.Id == commentUpdateDto.UserId);

                var errors = new List<string>();
                if (!adExists)
                    errors.Add($"Ad with ID {commentUpdateDto.AdId} does not exist");
                if (!userExists)
                    errors.Add($"User with ID {commentUpdateDto.UserId} does not exist");
                if (string.IsNullOrWhiteSpace(commentUpdateDto.Content))
                    errors.Add("Comment content cannot be empty");

                if (errors.Any())
                {
                    return CommentService<CommentReadDto>.BadResult(
                        "Validation failed",
                        400,
                        errors);
                }

                // Update properties
                existingComment.AdId = commentUpdateDto.AdId;
                existingComment.UserId = commentUpdateDto.UserId;
                existingComment.Content = commentUpdateDto.Content;
                existingComment.CreatedAt = commentUpdateDto.CreatedAt;

                _context.Entry(existingComment).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                // Load the updated comment with navigation properties
                var updatedComment = await _context.Comments
                    .Include(c => c.Ad)
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CommentId == id);

                var commentDto = new CommentReadDto
                {
                    CommentId = updatedComment!.CommentId,
                    AdId = updatedComment.AdId,
                    UserId = updatedComment.UserId,
                    CreatedAt = updatedComment.CreatedAt,
                    Content = updatedComment.Content,
                    Ad = updatedComment.Ad != null ? new AdBasicDto
                    {
                        AdId = updatedComment.Ad.AdId,
                        Title = updatedComment.Ad.Title,
                        Description = updatedComment.Ad.Description
                    } : null,
                    User = updatedComment.User != null ? new UserBasicDto
                    {
                        UserId = updatedComment.UserId,
                        Username = updatedComment.User.UserName,
                        Email = updatedComment.User.Email
                    } : null
                };

                return CommentService<CommentReadDto>.GoodResult(
                    "Comment updated successfully",
                    200,
                    commentDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CommentExistsAsync(id))
                {
                    return CommentService<CommentReadDto>.BadResult(
                        $"Comment with ID {id} not found",
                        404);
                }

                return CommentService<CommentReadDto>.BadResult(
                    "Concurrency conflict occurred while updating comment",
                    409);
            }
            catch (Exception ex)
            {
                return CommentService<CommentReadDto>.BadResult(
                    "Failed to update comment",
                    500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<CommentService<bool>> DeleteCommentAsync(int id)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(id);
                if (comment == null)
                {
                    return CommentService<bool>.BadResult(
                        $"Comment with ID {id} not found",
                        404,
                        data: false);
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return CommentService<bool>.GoodResult(
                    "Comment deleted successfully",
                    200,
                    true);
            }
            catch (Exception ex)
            {
                return CommentService<bool>.BadResult(
                    "Failed to delete comment",
                    500,
                    new List<string> { ex.Message },
                    false);
            }
        }

        public async Task<bool> CommentExistsAsync(int id)
        {
            return await _context.Comments.AnyAsync(e => e.CommentId == id);
        }
    }
}