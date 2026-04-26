using MediHub.Application.Interfaces;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class FacilityService : IFacilityService
    {
        private readonly IFacilityRepository _repository;

        public FacilityService(IFacilityRepository repository)
        {
            _repository = repository;
        }

        public Task<Facility> Create(Facility f)
        {
            return _repository.Create(f);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public Task<Facility> Update(Facility f)
        {
            return _repository.Update(f);
        }

        public async Task<IEnumerable<Facility>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Facility> GetById(int id)
        {
            return await _repository.GetById(id);
        }
    }
}

