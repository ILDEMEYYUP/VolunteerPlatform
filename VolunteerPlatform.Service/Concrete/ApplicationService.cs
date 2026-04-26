using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VolunteerPlatform.Data;
using VolunteerPlatform.Domain.Entities;
using VolunteerPlatform.Domain.Enums;
using VolunteerPlatform.Service.Interfaces;

namespace VolunteerPlatform.Service.Concrete
{
    public class ApplicationService : GenericService<Application>, IApplicationService
    {
        public ApplicationService(AppDbContext context) : base(context)
        {
        }

        public async Task ApplyToProjectAsync(Guid userId, Guid projectId, string coverLetter)
        {
            // Check if already applied
            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.VolunteerId == userId && a.ProjectId == projectId);
            
            if (existingApplication != null)
                throw new InvalidOperationException("You have already applied to this project.");

            var application = new Application
            {
                Id = Guid.NewGuid(),
                VolunteerId = userId,
                ProjectId = projectId,
                CoverLetter = coverLetter,
                AppliedAt = DateTime.UtcNow,
                Status = ApplicationStatus.Pending
            };

            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();
        }

        public async Task ReviewApplicationAsync(Guid applicationId, ApplicationStatus status)
        {
            var application = await _context.Applications.FindAsync(applicationId);
            if (application != null)
            {
                application.Status = status;
                _context.Applications.Update(application);
                await _context.SaveChangesAsync();
            }
        }
    }
}
