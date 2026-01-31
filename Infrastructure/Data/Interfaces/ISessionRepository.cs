using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface ISessionRepository
    {
        Task<int> Create(Session s);
        Task<int> Delete(int id);
        Task<int> Update(Session s);
        Task<IEnumerable<SessionDTO>> GetAll();
        Task<Session> GetById(int id);
    }
}
