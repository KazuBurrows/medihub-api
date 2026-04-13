using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface ISessionOverrideRepository
    {
        Task<SessionOverride> Create(int instanceId, SessionOverride s);
        Task Delete(int id);
        Task<SessionOverride> Update(SessionOverride s);

        Task<SessionOverride> GetById(int id);
        Task<SessionOverrideDTO> GetByIdDTO(int id);

        Task<int> getIdByInstanceId(int instanceId);
    }
}