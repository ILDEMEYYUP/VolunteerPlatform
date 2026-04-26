using System;

namespace VolunteerPlatform.Service.DTOs
{
    public class PortfolioSkillDto
    {
        public Guid SkillId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
