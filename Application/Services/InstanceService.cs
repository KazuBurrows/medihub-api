using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;
using MediHub.Infrastructure.Data.Utils;

namespace MediHub.Application.Services
{
    public class InstanceService : IInstanceService
    {
        private readonly IInstanceRepository _repository;

        public InstanceService(IInstanceRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(Instance s)
        {
            return _repository.Create(s);
        }

        public Task<int> Delete(int id)
        {
            return _repository.Delete(id);
        }

        public Task<int> Update(Instance s)
        {
            return _repository.Update(s);
        }

        public async Task<IEnumerable<Instance>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Instance> GetById(int id)
        {
            return await _repository.GetById(id);
        }

    }
}

