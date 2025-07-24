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
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesService _messagesService;

        public MessagesController(IMessagesService messagesService)
        {
            _messagesService = messagesService;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageReadDto>>> GetMessages()
        {
            var messages = await _messagesService.GetAllMessagesAsync();
            return Ok(messages);
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageReadDto>> GetMessage(int id)
        {
            var message = await _messagesService.GetMessageAsync(id);
            if (message == null)
                return NotFound();

            return Ok(message);
        }

        // POST: api/Messages
        [HttpPost]
        public async Task<ActionResult<MessageReadDto>> CreateMessage(MessageCreateDto messageCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdMessage = await _messagesService.CreateMessageAsync(messageCreateDto);
            return CreatedAtAction(nameof(GetMessage), new { id = createdMessage.MessageId }, createdMessage);
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMessage(int id, MessageUpdateDto messageUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != messageUpdateDto.MessageId)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                await _messagesService.UpdateMessageAsync(id, messageUpdateDto);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest("Message ID mismatch");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var deleted = await _messagesService.DeleteMessageAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}