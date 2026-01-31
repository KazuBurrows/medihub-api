using MediHub.Application.Interfaces;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _repository;

        public EquipmentService(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(Equipment e)
        {
            return _repository.Create(e);
        }

        public Task<int> Delete(int id)
        {
            return _repository.Delete(id);
        }

        public Task<int> Update(Equipment e)
        {
            return _repository.Update(e);
        }

        public async Task<IEnumerable<Equipment>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Equipment> GetById(int id)
        {
            return await _repository.GetById(id);
        }
        
    }
}

