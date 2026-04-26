using System;

namespace VolunteerPlatform.Service.DTOs
{
    public class PortfolioProjectDto
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Organizer or Volunteer
    }
}
