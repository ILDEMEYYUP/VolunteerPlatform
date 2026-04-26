using System;

namespace VolunteerPlatform.Domain.Entities;

public class Application
{
    public Guid Id { get; set; }
    
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    
    public Guid VolunteerId { get; set; }
    public User Volunteer { get; set; } = null!;
    
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public string CoverLetter { get; set; } = string.Empty;
    public VolunteerPlatform.Domain.Enums.ApplicationStatus Status { get; set; } = VolunteerPlatform.Domain.Enums.ApplicationStatus.Pending;


    /*

    Application{
        Id  : "Guid",
        ProjectId : "Guid",
        Project : "Project",
        VolunteerId : "Guid",
        Volunteer : "User",
        AppliedAt : "DateTime",
        CoverLetter : "string"
    }
    
    */
}
