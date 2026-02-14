using MediHub.Application.Interfaces;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(Role r)
        {
            return _repository.Create(r);
        }

        public Task<int> Delete(int id)
        {
            return _repository.Delete(id);
        }

        public Task<int> Update(Role r)
        {
            return _repository.Update(r);
        }

        public async Task<IEnumerable<Role>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Role> GetById(int id)
        {
            return await _repository.GetById(id);
        }
    }
}

