using Microsoft.AspNetCore.Mvc;
using VolunteerPlatform.Service.Interfaces;
using VolunteerPlatform.Service.DTOs.Response;

namespace VolunteerPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<SkillDto>>> GetTree()
        {
            var tree = await _skillService.GetSkillTreeAsync();
            return Ok(tree);
        }

        [HttpGet("roots")]
        public async Task<ActionResult<IEnumerable<SkillDto>>> GetRoots()
        {
            var roots = await _skillService.GetRootSkillsAsync();
            return Ok(roots);
        }

        [HttpGet("{parentId}/subs")]
        public async Task<ActionResult<IEnumerable<SkillDto>>> GetSubs(Guid parentId)
        {
            var subs = await _skillService.GetSubSkillsAsync(parentId);
            return Ok(subs);
        }
    }
}
