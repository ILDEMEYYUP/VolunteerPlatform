using System;
using System.Collections.Generic;
using VolunteerPlatform.Domain.Enums;

namespace VolunteerPlatform.Domain.Entities;

    // 
public class Project
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Pending;
    
    // Navigation properties
    public Guid OrganizerId { get; set; }
    public User Organizer { get; set; }= null!;
    
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<Skill> RequiredSkills { get; set; } = new List<Skill>();
}
