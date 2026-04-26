using VolunteerPlatform.Data;
using VolunteerPlatform.Domain.Entities;
using VolunteerPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using VolunteerPlatform.Domain.Enums; 
using VolunteerPlatform.Service.DTOs;


namespace VolunteerPlatform.Service.Concrete
{
    public class UserService : GenericService<User>, IUserService
    {
        protected readonly AppDbContext _context;
        
        public UserService(AppDbContext context) : base(context)
        {
            _context = context ; 
        }
        public async Task<ICollection<User>> getBySkillAsync(Skill requiredSkill)
        {
            return await _context.Users
                            .Include( u => u.Skills)
                            .Where(u => u.Skills.Any(s => s.Id == requiredSkill.Id))
                            .ToListAsync(); 
        }
        public async  Task<ICollection<User>>getByCompletedProjectAsync(Project project)
        {
            return await _context.Users
                                .Include(u => u.Projects )
                                .Where(u => u.Projects.Any(p => p.Id == project.Id  && p.Status == ProjectStatus.Completed))
                                .ToListAsync() ; 
        }
        public async Task UpdateUserSkillsAsync(System.Guid userId, List<System.Guid> skillIds)
        {
            var user = await _context.Users
                .Include(u => u.Skills)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new System.Exception("User not found");

            // Clear existing skills
            user.Skills.Clear();

            if (skillIds != null && skillIds.Any())
            {
                var skillsToAdd = await _context.Skills
                    .Where(s => skillIds.Contains(s.Id))
                    .ToListAsync();
                
                foreach (var skill in skillsToAdd)
                {
                    user.Skills.Add(skill);
                }
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<UserPortfolioDto> GetUserPortfolioAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Skills)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Applications)
                        .ThenInclude(a => a.Volunteer)
                .Include(u => u.Applications)
                    .ThenInclude(a => a.Project)
                        .ThenInclude(p => p.Applications)
                            .ThenInclude(a => a.Volunteer)
                .Include(u => u.Applications)
                    .ThenInclude(a => a.Project)
                        .ThenInclude(p => p.Organizer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var portfolio = new UserPortfolioDto
            {
                UserId = user.Id,
                FullName = $"{user.Name} {user.Surname}",
                
                Skills = user.Skills.Select(s => new PortfolioSkillDto
                {
                    SkillId = s.Id,
                    Name = s.Name
                }).ToList(),

                Projects = user.Projects.Select(p => new PortfolioProjectDto
                {
                    ProjectId = p.Id,
                    Title = p.Title,
                    Status = p.Status.ToString(),
                    Role = "Organizer"
                }).ToList()
            };

            var acceptedApplications = user.Applications
                .Where(a => a.Status == ApplicationStatus.Accepted).ToList();

            foreach (var app in acceptedApplications)
            {
                portfolio.Projects.Add(new PortfolioProjectDto
                {
                    ProjectId = app.ProjectId,
                    Title = app.Project.Title,
                    Status = app.Project.Status.ToString(),
                    Role = "Volunteer"
                });
            }

            portfolio.Applications = user.Applications.Select(a => new PortfolioApplicationDto
            {
                ApplicationId = a.Id,
                ProjectTitle = a.Project.Title,
                ApplicationStatus = a.Status.ToString()
            }).ToList();

            var teammates = new List<PortfolioTeammateDto>();

            // Teammates from user's own projects
            foreach (var proj in user.Projects)
            {
                var volunteers = proj.Applications
                    .Where(a => a.Status == ApplicationStatus.Accepted && a.VolunteerId != userId)
                    .Select(a => new PortfolioTeammateDto
                    {
                        TeammateId = a.VolunteerId,
                        FullName = $"{a.Volunteer.Name} {a.Volunteer.Surname}",
                        ProjectTitle = proj.Title,
                        ProjectStatus = proj.Status.ToString()
                    });
                teammates.AddRange(volunteers);
            }

            // Teammates from projects where user is a volunteer
            foreach (var app in acceptedApplications)
            {
                var otherVolunteers = app.Project.Applications
                    .Where(a => a.Status == ApplicationStatus.Accepted && a.VolunteerId != userId)
                    .Select(a => new PortfolioTeammateDto
                    {
                        TeammateId = a.VolunteerId,
                        FullName = $"{a.Volunteer.Name} {a.Volunteer.Surname}",
                        ProjectTitle = app.Project.Title,
                        ProjectStatus = app.Project.Status.ToString()
                    });
                
                teammates.AddRange(otherVolunteers);

                if (app.Project.OrganizerId != userId && app.Project.Organizer != null)
                {
                    teammates.Add(new PortfolioTeammateDto
                    {
                        TeammateId = app.Project.OrganizerId,
                        FullName = $"{app.Project.Organizer.Name} {app.Project.Organizer.Surname}",
                        ProjectTitle = app.Project.Title,
                        ProjectStatus = app.Project.Status.ToString()
                    });
                }
            }

            // distinct by teammate and project
            portfolio.Teammates = teammates
                .GroupBy(t => new { t.TeammateId, t.ProjectTitle })
                .Select(g => g.First())
                .ToList();
            
            return portfolio;
        }
    }
}

