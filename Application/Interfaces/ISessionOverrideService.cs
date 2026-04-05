using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ISessionOverrideService
    {
        Task<SessionOverride> Create(SessionOverride s);
        Task Delete(int id);
        Task<SessionOverride> Update(SessionOverride s);

        Task<SessionOverride> GetById(int id);
        Task<SessionOverrideDTO> GetByIdDTO(int id);

    }
}