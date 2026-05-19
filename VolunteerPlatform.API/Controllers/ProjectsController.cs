using Microsoft.AspNetCore.Mvc;
using VolunteerPlatform.Service.Interfaces;
using VolunteerPlatform.Service.DTOs.Response;
using VolunteerPlatform.Service.DTOs.Request;
using VolunteerPlatform.Domain.Entities;
using VolunteerPlatform.Domain.Enums;

namespace VolunteerPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IProjectDiscoveryService _discoveryService;

        public ProjectsController(IProjectService projectService, IProjectDiscoveryService discoveryService)
        {
            _projectService = projectService;
            _discoveryService = discoveryService;
        }

        // GET api/projects — Ham entity listesi (admin vb. amaçlı)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            var projects = await _projectService.GetAllAsync();
            return Ok(projects);
        }

        // GET api/projects/details — DTO listesi (keşif/liste ekranı)
        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<ProjectDetailDto>>> GetAllWithDetails()
        {
            var projects = await _projectService.GetAllWithDetailsAsync();
            return Ok(projects);
        }

        // GET api/projects/{id}/details — Tek proje detayı
        [HttpGet("{id}/details")]
        public async Task<ActionResult<ProjectDetailDto>> GetDetails(Guid id)
        {
            var project = await _projectService.GetDetailByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        // GET api/projects/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> Get(Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        // POST api/projects
        [HttpPost]
        public async Task<ActionResult<Project>> Create([FromBody] ProjectCreateDto dto)
        {
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                CorrelationId = projectId,
                Title = dto.Title,
                Description = dto.Description,
                ActualEndDate = dto.ActualEndDate,
                LeaderId = dto.LeaderId,
                Status = ProjectStatus.Pending,
                ProjectCreatedAt = DateTime.UtcNow
            };

            await _projectService.AddAsync(project);

            if (dto.RequiredSkillIds != null && dto.RequiredSkillIds.Any())
                await _projectService.AssignSkillsToProjectAsync(project.CorrelationId, dto.RequiredSkillIds);

            return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
        }

        // PUT api/projects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProjectUpdateDto dto)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound();

            await _projectService.UpdateProjectAsync(project, dto);
            return NoContent();
        }

        // DELETE api/projects/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound();

            await _projectService.DeleteAsync(id);
            return NoContent();
        }

        // DELETE api/projects/{projectId}/user/{userId}
        [HttpDelete("{projectId}/user/{userId}")]
        public async Task<IActionResult> RemoveUserFromProject(Guid projectId, Guid userId)
        {
            var result = await _projectService.RemoveUserFromProjectAsync(projectId, userId);
            if (!result) return NotFound();
            return NoContent();
        }
        // POST api/projects/{projectId}/accept-user/{userId}
        [HttpPost("{projectId}/accept-user/{userId}")]
        public async Task<IActionResult> AcceptUserIntoProject(Guid projectId, Guid userId)
        {
            // IProjectService üzerinden senin yazdığın SCD uyumlu AddUserToProjectAsync metodunu çağırıyoruz
            var result = await _projectService.AddUserToProjectAsync(projectId, userId);
            
            if (!result) 
                return BadRequest("Başvuru onaylanırken bir hata oluştu veya başvuru bulunamadı.");

            // Başarıyla onaylandıysa 200 OK dönüyoruz, böylece frontend sonraki adıma (SignalR mesajına) geçebilecek
            return Ok(new { Success = true, Message = "Kullanıcı başarıyla projeye kabul edildi." });
        }
        // POST api/projects/{id}/close
        [HttpPost("{id}/close")]
        public async Task<IActionResult> Close(Guid id)
        {
            await _projectService.CloseProjectAsync(id);
            return NoContent();
        }

        // ─── Discovery Endpoints ──────────────────────────────────────────────────

        // GET api/projects/recommended/{userId}
        [HttpGet("recommended/{userId}")]
        public async Task<ActionResult<PagedResult<ProjectDetailDto>>> GetRecommended(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _discoveryService.GetRecommendedProjectsAsync(userId, page, pageSize);
            return Ok(result);
        }

        // GET api/projects/popular
        [HttpGet("popular")]
        public async Task<ActionResult<PagedResult<ProjectDetailDto>>> GetPopular(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _discoveryService.GetPopularProjectsAsync(page, pageSize);
            return Ok(result);
        }

        // GET api/projects/search?keyword=...&skillIds=...
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProjectDetailDto>>> Search(
            [FromQuery] string? keyword,
            [FromQuery] List<Guid>? skillIds)
        {
            var result = await _discoveryService.SearchAndFilterProjectsAsync(
                keyword ?? string.Empty,
                skillIds ?? new List<Guid>());
            return Ok(result);
        }

        // GET api/projects/search-by-skills?skillIds=...
        [HttpGet("search-by-skills")]
        public async Task<ActionResult<IEnumerable<ProjectDetailDto>>> SearchBySkills(
            [FromQuery] List<Guid> skillIds)
        {
            var result = await _discoveryService.SearchProjectsBySkillsAsync(skillIds);
            return Ok(result);
        }

        // GET api/projects/search-by-title?keyword=...
        [HttpGet("search-by-title")]
        public async Task<ActionResult<IEnumerable<ProjectDetailDto>>> SearchByTitle(
            [FromQuery] string keyword)
        {
            var result = await _discoveryService.SearchProjectsByTitleAsync(keyword);
            return Ok(result);
        }
    }
}
