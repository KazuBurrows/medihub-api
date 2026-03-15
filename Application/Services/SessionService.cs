using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _repository;

        public SessionService(ISessionRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(Session s)
        {
            return _repository.Create(s);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public Task<int> Update(Session s)
        {
            return _repository.Update(s);
        }

        public async Task<IEnumerable<Session>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<IEnumerable<SessionDTO>> GetAllDTO()
        {
            return await _repository.GetAllDTO();
        }

        public async Task<Session> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task<SessionDTO> GetByIdDTO(int id)
        {
            return await _repository.GetByIdDTO(id);
        }
    }
}

