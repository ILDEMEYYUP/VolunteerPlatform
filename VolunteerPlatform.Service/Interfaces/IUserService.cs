using VolunteerPlatform.Domain.Entities;
using VolunteerPlatform.Service.DTOs ; 

namespace VolunteerPlatform.Service.Interfaces
{
    public interface IUserService : IGenericService<User>
    {
        Task<ICollection<User>> getBySkillAsync(Skill requiredSkill); 
        Task<ICollection<User>>getByCompletedProjectAsync(Project project); 
        Task<UserPortfolioDto> GetUserPortfolioAsync(Guid userId);
        Task UpdateUserSkillsAsync(Guid userId, List<Guid> skillIds);
    }
}
