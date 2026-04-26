using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface ISessionRepository
    {
        Task<Session> Create(Session s);
        Task Delete(int id);
        Task<Session> Update(Session s);
        Task<IEnumerable<Session>> GetAll();
        Task<IEnumerable<SessionDTO>> GetAllDTO();
        Task<Session> GetById(int id);
        Task<SessionDTO> GetByIdDTO(int id);
    }
}
