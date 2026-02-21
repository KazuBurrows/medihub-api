using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ISessionService
    {
        Task<int> Create(Session s);
        Task<int> Delete(int id);
        Task<int> Update(Session s);
        Task<IEnumerable<Session>> GetAll();

        Task<IEnumerable<SessionDTO>> GetAllDTO();
        Task<Session> GetById(int id);
        Task<SessionDTO> GetByIdDTO(int id);

    }
}