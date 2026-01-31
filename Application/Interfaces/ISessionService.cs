using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ISessionService
    {
        Task<int> Create(Session s);
        Task<int> Delete(int id);
        Task<int> Update(Session s);
        Task<IEnumerable<SessionDTO>> GetAll();
        Task<Session> GetById(int id);

        // Task<SessionDTO> GetAggById(int id);
    }
}