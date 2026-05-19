using Microsoft.AspNetCore.Mvc;
using VolunteerPlatform.Service.Interfaces;
using VolunteerPlatform.Service.DTOs.Request;
using VolunteerPlatform.Service.DTOs.Response;
using VolunteerPlatform.Domain.Entities;

namespace VolunteerPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        // GET api/applications/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Application>> Get(Guid id)
        {
            var application = await _applicationService.GetByIdAsync(id);
            if (application == null) return NotFound();
            return Ok(application);
        }

        // GET api/applications/project/{projectId}
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetByProject(Guid projectId)
        {
            var applications = await _applicationService.GetApplicationsByProjectIdAsync(projectId);
            return Ok(applications);
        }

        // POST api/applications
        [HttpPost]
        public async Task<IActionResult> Apply([FromBody] ApplicationCreateDto dto)
        {
            await _applicationService.ApplyToProjectAsync(dto.VolunteerCorrelationId, dto.ProjectCorrelationId, dto.CoverLetter);
            return Ok(new { message = "Application submitted successfully." });
        }

        // POST api/applications/{id}/review
        [HttpPost("{id}/review")]
        public async Task<IActionResult> Review(Guid id, [FromBody] ApplicationReviewDto dto)
        {
            // dto.Status zaten ApplicationStatus tipinde — cast gerekmez
            await _applicationService.ReviewApplicationAsync(id, dto.Status);
            return NoContent();
        }

        // DELETE api/applications/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _applicationService.DeleteAsync(id);
            return NoContent();
        }
    }
}
