using System;
using System.Collections.Generic;
using VolunteerPlatform.Domain.Enums;

namespace VolunteerPlatform.Domain.Entities;

// eklenebilcek özellikler  : 
// profil fotoğrafı fakat acesi yok ayrıca yaş ve cinsihyet de olabilr
// fakat ne kadar mantıklı  hocaya sormak lazım 
// 
public class User
{
    // temel bilgiler
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; }
    
    // Education Info
    public string Department { get; set; } = string.Empty;
    public int? EnrollmentYear { get; set; }
    
    // Links
    public string LinkedInUrl { get; set; } = string.Empty;
    public string GithubUrl { get; set; } = string.Empty;
    public string PortfolioUrl { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    
    // Matching Service Support
    public ICollection<Project> RecommendedProjects { get; set; } = new List<Project>();
    
    // message support
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
}