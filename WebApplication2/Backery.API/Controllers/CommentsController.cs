using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services.Interfaces;


namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentReadDto>>> GetComments()
        {
            var comments = await _commentsService.GetAllCommentsAsync();
            return Ok(comments);
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentReadDto>> GetComment(int id)
        {
            var comment = await _commentsService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }

        // POST: api/Comments
        [HttpPost]
        public async Task<ActionResult<CommentReadDto>> CreateComment(CommentCreateDto commentCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdComment = await _commentsService.CreateCommentAsync(commentCreateDto);
            return CreatedAtAction(nameof(GetComment), new { id = createdComment.CommentId }, createdComment);
        }

        // PUT: api/Comments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, CommentUpdateDto commentUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != commentUpdateDto.CommentId)
            {
                return BadRequest("ID mismatch");
            }

            var result = await _commentsService.UpdateCommentAsync(id, commentUpdateDto);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var result = await _commentsService.DeleteCommentAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
