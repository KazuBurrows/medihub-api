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

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
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

        public async Task<Staff> GetByEmail(string email)
        {
            return await _repository.GetByEmail(email);
        }

        public async Task<IEnumerable<StaffDTO>> GetAllDTO()
        {
            return await _repository.GetAllDTO();
        }

        public async Task<StaffDTO> GetByIdDTO(int id)
        {
            return await _repository.GetByIdDTO(id);
        }
    }
}

