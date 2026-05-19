using Microsoft.AspNetCore.Mvc;
using VolunteerPlatform.Service.DTOs.Response;
using VolunteerPlatform.Service.Interfaces;

namespace VolunteerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMyNotifications([FromHeader(Name = "X-User-Id")] Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("X-User-Id header is required.");
        }

        var notifications = await _notificationService.GetUserNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("X-User-Id header is required.");
        }

        await _notificationService.MarkAsReadAsync(id, userId);
        return NoContent();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead([FromHeader(Name = "X-User-Id")] Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("X-User-Id header is required.");
        }

        await _notificationService.MarkAllAsReadAsync(userId);
        return NoContent();
    }
}
