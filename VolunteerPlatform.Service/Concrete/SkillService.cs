using VolunteerPlatform.Data;
using VolunteerPlatform.Domain.Entities;
using VolunteerPlatform.Service.Interfaces;

namespace VolunteerPlatform.Service.Concrete
{
    public class SkillService : GenericService<Skill>, ISkillService
    {
        public SkillService(AppDbContext context) : base(context)
        {
        }
    }
}
