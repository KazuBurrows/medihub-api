using MediHub.Application.Interfaces;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ISpecialtyRepository _repository;

        public SpecialtyService(ISpecialtyRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(Specialty s)
        {
            return _repository.Create(s);
        }

        public Task<int> Delete(int id)
        {
            return _repository.Delete(id);
        }

        public Task<int> Update(Specialty s)
        {
            return _repository.Update(s);
        }

        public async Task<IEnumerable<Specialty>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Specialty> GetById(int id)
        {
            return await _repository.GetById(id);
        }
    }
}

