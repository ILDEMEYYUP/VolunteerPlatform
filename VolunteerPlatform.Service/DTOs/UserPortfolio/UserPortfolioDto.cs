namespace VolunteerPlatform.Service.DTOs
{
    public class UserPortfolioDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        
        public List<PortfolioProjectDto> Projects { get; set; } = new();
        public List<PortfolioSkillDto> Skills { get; set; } = new();
        public List<PortfolioTeammateDto> Teammates { get; set; } = new();
        public List<PortfolioApplicationDto> Applications { get; set; } = new();
    }
}
