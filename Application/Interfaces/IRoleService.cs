using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IRoleService
    {
        Task<Role> Create(Role r);
        Task Delete(int id);
        Task<Role> Update(Role r);
        Task<IEnumerable<Role>> GetAll();
        Task<Role> GetById(int id);
    }
}