using MediHub.Application.Interfaces;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class SurgeonTypeService : ISurgeonTypeService
    {
        private readonly ISurgeonTypeRepository _repository;

        public SurgeonTypeService(ISurgeonTypeRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(SurgeonType s)
        {
            return _repository.Create(s);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public Task<int> Update(SurgeonType s)
        {
            return _repository.Update(s);
        }

        public async Task<IEnumerable<SurgeonType>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<SurgeonType> GetById(int id)
        {
            return await _repository.GetById(id);
        }
    }
}

