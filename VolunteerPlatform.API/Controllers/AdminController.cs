using Microsoft.AspNetCore.Mvc;
using VolunteerPlatform.Service.Interfaces;
using VolunteerPlatform.Service.DTOs.Request;
using VolunteerPlatform.Service.DTOs.Response;
using VolunteerPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using VolunteerPlatform.Data;
using VolunteerPlatform.Domain.Entities;

namespace VolunteerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")] // TODO: Auth eklendiğinde açılacak
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IProjectService _projectService;
    private readonly IReportService _reportService;
    private readonly INotificationService _notificationService;
    private readonly ISkillService _skillService;
    private readonly AppDbContext _context;

    public AdminController(
        IUserService userService,
        IProjectService projectService,
        IReportService reportService,
        INotificationService notificationService,
        ISkillService skillService,
        AppDbContext context)
    {
        _userService = userService;
        _projectService = projectService;
        _reportService = reportService;
        _notificationService = notificationService;
        _skillService = skillService;
        _context = context;
    }

    // 1. Kullanıcılara bildirim gönderebilme
    [HttpPost("notifications")]
    public async Task<IActionResult> SendNotification([FromBody] CreateNotificationRequestDto request)
    {
        await _notificationService.CreateNotificationAsync(request);
        return Ok(new { message = "Bildirim gönderildi." });
    }

    [HttpPost("notifications/bulk")]
    public async Task<IActionResult> SendBulkNotification([FromBody] BulkNotificationRequestDto request)
    {
        await _notificationService.SendBulkNotificationAsync(request.RecipientIds, request.Title, request.Message, request.Type);
        return Ok(new { message = "Toplu bildirim gönderildi." });
    }

    // 2. Gelen şikayetleri görüntüleyebilme
    [HttpGet("reports")]
    public async Task<ActionResult<IEnumerable<ReportDto>>> GetReports()
    {
        var reports = await _reportService.GetAllReportsAsync();
        return Ok(reports);
    }

    [HttpPut("reports/{id}/resolve")]
    public async Task<IActionResult> ResolveReport(Guid id)
    {
        await _reportService.ResolveReportAsync(id);
        return NoContent();
    }

    // 3. Projeyi kaldırabilme
    [HttpDelete("projects/{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var project = await _projectService.GetByCorrelationIdAsync(id);
        if (project == null) return NotFound();

        await _projectService.DeleteAsync(id);
        return NoContent();
    }

    // 4. Kullanıcıyı banlayabilme (Delete ile)
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> BanUser(Guid id)
    {
        var user = await _userService.GetByCorrelationIdAsync(id);
        if (user == null) return NotFound();

        await _userService.DeleteAsync(id);
        return NoContent();
    }

    // 5. Kullanıcı ve proje sayısını görüntüleyebilme
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var userCount = await _context.Users.CountAsync(u => u.IsLatest);
        var projectCount = await _context.Projects.CountAsync(p => p.IsLatest);
        var reportCount = await _context.Reports.CountAsync(r => !r.IsResolved && r.IsLatest);

        return Ok(new
        {
            TotalUsers = userCount,
            TotalProjects = projectCount,
            PendingReports = reportCount
        });
    }

    // 6. Yetenek ağacına yetenek ekleyebilme
    [HttpPost("skills")]
    public async Task<IActionResult> AddSkill([FromBody] SkillCreateDto request)
    {
        Guid? actualParentId = null;
        if (request.ParentId.HasValue && request.ParentId.Value != Guid.Empty)
        {
            var parentSkill = await _context.Skills
                .FirstOrDefaultAsync(s => s.CorrelationId == request.ParentId.Value && s.IsLatest);
            if (parentSkill != null)
            {
                actualParentId = parentSkill.Id;
            }
        }

        var skill = new Skill
        {
            Id = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ParentId = actualParentId,
            IsApproved = true // Admin eklediği için onaylı başlasın
        };

        await _skillService.AddAsync(skill);
        
        return Ok(new SkillDto
        {
            SkillCorrelationId = skill.CorrelationId,
            Name = skill.Name,
            Description = skill.Description,
            ParentId = request.ParentId,
            SubSkills = new List<SkillDto>()
        });
    }
}

public class BulkNotificationRequestDto
{
    public List<Guid> RecipientIds { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
}

public class SkillCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
}
