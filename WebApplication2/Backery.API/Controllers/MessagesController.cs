using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DTO;
using WebApplication2.Properties.Data;
using WebApplication2.Properties.DTOs;
using WebApplication2.Properties.Models;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesService _messagesService;

        public MessagesController(IMessagesService messagesService)
        {
            _messagesService = messagesService;
        }

        // GET: api/Messages
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MessageReadDto>>> GetMessages()
        {
            var result = await _messagesService.GetAllMessagesAsync();
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });

        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<MessageReadDto>> GetMessage(int id)
        {
            var result = await _messagesService.GetMessageAsync(id);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }


        // POST: api/Messages
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MessageReadDto>> CreateMessage(MessageCreateDto messageCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Model validation failed",
                    errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var result = await _messagesService.CreateMessageAsync(messageCreateDto);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateMessage(int id, MessageUpdateDto messageUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Model validation failed",
                    errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            }
            var result = await _messagesService.UpdateMessageAsync(id, messageUpdateDto);
            if (result.Success)
                return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });

        }
        

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var result = await _messagesService.DeleteMessageAsync(id);
            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }
    }
}