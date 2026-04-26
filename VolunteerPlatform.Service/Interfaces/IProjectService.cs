using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VolunteerPlatform.Domain.Entities;

namespace VolunteerPlatform.Service.Interfaces
{
    public interface IProjectService : IGenericService<Project>
    {
        Task<IEnumerable<Project>> GetRecommendedProjectsAsync(Guid userId);
        Task<IEnumerable<Project>> SearchAndFilterProjectsAsync(string keyword, List<Guid> skillIds);
        Task CloseProjectAsync(Guid projectId);
    }
}
