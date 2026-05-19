using Microsoft.AspNetCore.Mvc;
using VolunteerPlatform.Service.Interfaces;
using VolunteerPlatform.Service.DTOs.Response;
using VolunteerPlatform.Service.DTOs.Request;
using VolunteerPlatform.Service.DTOs;
using VolunteerPlatform.Domain.Entities;


namespace VolunteerPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST api/users
        [HttpPost]
        public async Task<ActionResult<UserSummaryDto>> Create([FromBody] UserCreateDto createDto)
        {
            var userId = Guid.NewGuid();
            var newUser = new User
            {
                Id = userId,
                CorrelationId = userId,
                Name = createDto.Name,
                Surname = createDto.Surname,
                Email = createDto.Email,
                Department = createDto.Department ?? string.Empty,
                Role = createDto.Role
            };
            await _userService.AddAsync(newUser);
            return CreatedAtAction(nameof(Get), new { id = newUser.Id },
                new { newUser.Id, newUser.CorrelationId, newUser.Name, newUser.Surname });
        }

        // PUT api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto updateDto)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = updateDto.Name ?? existing.Name;
            existing.Surname = updateDto.Surname ?? existing.Surname;
            existing.Email = updateDto.Email ?? existing.Email;
            existing.Department = updateDto.Department ?? existing.Department;

            if (updateDto.Role.HasValue)
                existing.Role = updateDto.Role.Value;

            await _userService.UpdateAsync(existing);
            return NoContent();
        }

        // DELETE api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        // GET api/users/{id}/portfolio
        [HttpGet("{id}/portfolio")]
        public async Task<ActionResult<UserPortfolioDto>> GetPortfolio(Guid id)
        {
            var portfolio = await _userService.GetUserPortfolioAsync(id);
            if (portfolio == null) return NotFound();
            return Ok(portfolio);
        }

        // POST api/users/{id}/skills
        [HttpPost("{id}/skills")]
        public async Task<IActionResult> UpdateSkills(Guid id, [FromBody] List<Guid> skillIds)
        {
            await _userService.UpdateUserSkillsAsync(id, skillIds);
            return NoContent();
        }
        
        [HttpDelete("{id}/skills/{skillId}")]
        public async Task<IActionResult> RemoveSkill(Guid id, Guid skillId)
        {
            await _userService.RemoveUserSkillAsync(id, skillId);
            return NoContent(); 
        }
    }
}
