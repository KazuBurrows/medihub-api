using MediHub.Application.Interfaces;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repository;

        public StaffService(IStaffRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(Staff s)
        {
            return _repository.Create(s);
        }

        public Task<int> Delete(int id)
        {
            return _repository.Delete(id);
        }

        public Task<int> Update(Staff s)
        {
            return _repository.Update(s);
        }

        public async Task<IEnumerable<Staff>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Staff> GetById(int id)
        {
            return await _repository.GetById(id);
        }
    }
}

