using System.Collections.Generic;
using System.Threading.Tasks;

namespace VolunteerPlatform.Service.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        Task<T> GetByIdAsync(System.Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(System.Guid id);
    }
}
