using MediHub.Application.Interfaces;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class AnaestheticTypeService : IAnaestheticTypeService
    {
        private readonly IAnaestheticTypeRepository _repository;

        public AnaestheticTypeService(IAnaestheticTypeRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(AnaestheticType s)
        {
            return _repository.Create(s);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public Task<int> Update(AnaestheticType s)
        {
            return _repository.Update(s);
        }

        public async Task<IEnumerable<AnaestheticType>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<AnaestheticType> GetById(int id)
        {
            return await _repository.GetById(id);
        }
    }
}

