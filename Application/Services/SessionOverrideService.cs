using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class SessionOverrideService : ISessionOverrideService
    {
        private readonly ISessionOverrideRepository _repository;

        public SessionOverrideService(ISessionOverrideRepository repository)
        {
            _repository = repository;
        }

        public Task<SessionOverride> Create(int instanceId, SessionOverride s)
        {
            return _repository.Create(instanceId, s);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public Task<SessionOverride> Update(SessionOverride s)
        {
            return _repository.Update(s);
        }


        public async Task<SessionOverride> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task<SessionOverrideDTO> GetByIdDTO(int id)
        {
            return await _repository.GetByIdDTO(id);
        }

        public async Task<int> getIdByInstanceId(int instanceId)
        {
            return await _repository.getIdByInstanceId(instanceId);
        }
    }
}

