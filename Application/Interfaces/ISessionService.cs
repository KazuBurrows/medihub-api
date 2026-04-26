using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ISessionService
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