using Microsoft.AspNetCore.Mvc;
using VolunteerPlatform.Service.DTOs.Request;
using VolunteerPlatform.Service.Interfaces;

namespace VolunteerPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Basit login mantığı: Kullanıcı adı veya email ile ara, bulursan bilgileri dön.
            // Şifre kontrolü şu anlık test süreci için yok.
            var user = await _userService.LoginAsync(request.Username);

            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }

            return Ok(user);
        }
    }
}
