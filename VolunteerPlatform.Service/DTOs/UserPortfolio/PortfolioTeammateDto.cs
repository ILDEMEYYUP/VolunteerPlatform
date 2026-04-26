using System;

namespace VolunteerPlatform.Service.DTOs
{
    public class PortfolioTeammateDto
    {
        public Guid TeammateId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string ProjectStatus { get; set; } = string.Empty;
    }
}
