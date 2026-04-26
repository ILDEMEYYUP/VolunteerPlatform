using System;
using System.Threading.Tasks;
using VolunteerPlatform.Domain.Entities;
using VolunteerPlatform.Domain.Enums;

namespace VolunteerPlatform.Service.Interfaces
{
    public interface IApplicationService : IGenericService<Application>
    {
        Task ApplyToProjectAsync(Guid userId, Guid projectId, string coverLetter);
        Task ReviewApplicationAsync(Guid applicationId, ApplicationStatus status);
    }
}
