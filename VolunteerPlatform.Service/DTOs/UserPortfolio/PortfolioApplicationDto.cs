using System;

namespace VolunteerPlatform.Service.DTOs
{
    public class PortfolioApplicationDto
    {
        public Guid ApplicationId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string ApplicationStatus { get; set; } = string.Empty;
    }
}
