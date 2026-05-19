using Microsoft.AspNetCore.Mvc;
using VolunteerPlatform.Service.Interfaces;
using VolunteerPlatform.Service.DTOs.Response;
using VolunteerPlatform.Service.DTOs.Request;

namespace VolunteerPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // GET: api/Messages/conversation?user1Id=...&user2Id=...
        [HttpGet("conversation")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetConversation([FromQuery] Guid user1Id, [FromQuery] Guid user2Id)
        {
            var messages = await _messageService.GetConversationAsync(user1Id, user2Id);
            return Ok(messages);
        }

        // GET: api/Messages/inbox?userId=...
        [HttpGet("inbox")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetInbox([FromQuery] Guid userId)
        {
            var inbox = await _messageService.GetInboxAsync(userId);
            return Ok(inbox);
        }

        // POST: api/Messages
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] MessageCreateDto dto)
        {
            await _messageService.SendMessageAsync(dto.SenderId, dto.ReceiverId, dto.Content);
            return Ok(new { message = "Message sent" });
        }

        // POST: api/Messages/{id}/read
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _messageService.MarkAsReadAsync(id);
            return NoContent();
        }

        // DELETE: api/Messages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _messageService.SoftDeleteMessageAsync(id);
            return NoContent();
        }
    }
}