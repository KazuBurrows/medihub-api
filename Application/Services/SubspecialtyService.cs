using MediHub.Application.Interfaces;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class SubspecialtyService : ISubspecialtyService
    {
        private readonly ISubspecialtyRepository _repository;

        public SubspecialtyService(ISubspecialtyRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(Subspecialty s)
        {
            return _repository.Create(s);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public Task<int> Update(Subspecialty s)
        {
            return _repository.Update(s);
        }

        public async Task<IEnumerable<Subspecialty>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Subspecialty> GetById(int id)
        {
            return await _repository.GetById(id);
        }
    }
}

