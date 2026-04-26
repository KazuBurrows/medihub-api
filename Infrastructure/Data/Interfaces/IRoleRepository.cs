using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> Create(Role r);
        Task Delete(int id);
        Task<Role> Update(Role r);
        Task<IEnumerable<Role>> GetAll();
        Task<Role> GetById(int id);
    }
}
