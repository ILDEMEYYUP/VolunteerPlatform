using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolunteerPlatform.Service.Interfaces;

namespace VolunteerPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;

        public MediaController(IMediaService mediaService, IUserService userService, IProjectService projectService)
        {
            _mediaService = mediaService;
            _userService = userService;
            _projectService = projectService;
        }

        // 1. Profil Fotoğrafı Yükleme
        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file, [FromHeader(Name = "X-User-Id")] Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest("X-User-Id gereklidir.");

            var user = await _userService.GetByCorrelationIdAsync(userId);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            try
            {
                var filePath = await _mediaService.UploadFileAsync(file, "profiles");
                
                // Kullanıcıyı güncelle
                user.ProfilePictureUrl = $"/api/media/get?path={filePath}";
                await _userService.UpdateAsync(user);

                return Ok(new { url = user.ProfilePictureUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 2. Proje Fotoğrafı Yükleme
        [HttpPost("upload-project-picture/{projectId}")]
        public async Task<IActionResult> UploadProjectPicture(Guid projectId, IFormFile file)
        {
            var project = await _projectService.GetByCorrelationIdAsync(projectId);
            if (project == null) return NotFound("Proje bulunamadı.");

            try
            {
                var filePath = await _mediaService.UploadFileAsync(file, "projects");
                
                // Projeyi güncelle
                project.ProjectImageUrl = $"/api/media/get?path={filePath}";
                await _projectService.UpdateAsync(project);

                return Ok(new { url = project.ProjectImageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 3. File Proxy - Dosyayı Servis Etme
        [HttpGet("get")]
        public async Task<IActionResult> GetFile([FromQuery] string path)
        {
            try
            {
                var (content, contentType, fileName) = await _mediaService.GetFileAsync(path);
                return File(content, contentType);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
