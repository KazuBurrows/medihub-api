using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IRoleService
    {
        Task<int> Create(Role r);
        Task Delete(int id);
        Task<int> Update(Role r);
        Task<IEnumerable<Role>> GetAll();
        Task<Role> GetById(int id);
    }
}