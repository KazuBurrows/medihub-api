using MediHub.Application.Interfaces;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;
using Version = MediHub.Domain.Models.Version;

namespace MediHub.Application.Services
{
    public class VersionService : IVersionService
    {
        private readonly IVersionRepository _repository;

        public VersionService(IVersionRepository repository)
        {
            _repository = repository;
        }

        public Task<Version> Create(Version v)
        {
            return _repository.Create(v);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public Task<Version> Update(Version v)
        {
            return _repository.Update(v);
        }

        public async Task<IEnumerable<Version>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Version> GetById(int id)
        {
            return await _repository.GetById(id);
        }
    }
}

