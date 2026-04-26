using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VolunteerPlatform.Data;
using VolunteerPlatform.Domain.Entities;
using VolunteerPlatform.Domain.Enums;
using VolunteerPlatform.Service.Interfaces;

namespace VolunteerPlatform.Service.Concrete
{
    public class ProjectService : GenericService<Project>, IProjectService
    {
        public ProjectService(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Project>> GetRecommendedProjectsAsync(Guid userId)
        {
            var user = await _context.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || !user.Skills.Any())
                return new List<Project>();

            var userSkillIds = user.Skills.Select(s => s.Id).ToList();

            var recommendedProjects = await _context.Projects
                .Include(p => p.RequiredSkills)
                .Where(p => p.Status == ProjectStatus.Pending && p.RequiredSkills.Any(rs => userSkillIds.Contains(rs.Id)))
                .ToListAsync();

            return recommendedProjects;
        }

        public async Task<IEnumerable<Project>> SearchAndFilterProjectsAsync(string keyword, List<Guid> skillIds)
        {
            var query = _context.Projects.Include(p => p.RequiredSkills).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.Title.Contains(keyword) || p.Description.Contains(keyword));
            }

            if (skillIds != null && skillIds.Any())
            {
                query = query.Where(p => p.RequiredSkills.Any(rs => skillIds.Contains(rs.Id)));
            }

            return await query.Where(p => p.Status == ProjectStatus.Pending).ToListAsync();
        }

        public async Task CloseProjectAsync(Guid projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project != null)
            {
                project.Status = ProjectStatus.Completed;
                project.EndDate = DateTime.UtcNow;
                _context.Projects.Update(project);
                await _context.SaveChangesAsync();
            }
        }
    }
}
