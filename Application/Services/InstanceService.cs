using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Domain.Matrix;
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

        public Task<Instance> Create(Instance s)
        {
            return _repository.Create(s);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public Task<Instance> Update(Instance s)
        {
            return _repository.Update(s);
        }

        public async Task<IEnumerable<Instance>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<IEnumerable<InstanceDTO>> GetAllDTOByDate(string startDate, string endDate, int versionId)
        {
            return await _repository.GetAllDTOByDate(startDate, endDate, versionId);
        }

        public async Task<Instance> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task<IEnumerable<InstanceDTO>> GetAllByStaffId(int staffId)
        {
            return await _repository.GetAllByStaffId(staffId);
        }

        public async Task<IEnumerable<InstanceDTO>> GetAllDTO()
        {
            return await _repository.GetAllDTO();
        }

        public async Task<InstanceDTO> CreateDTO(InstanceDTO i)
        {
            return await _repository.CreateDTO(i);
        }

        public async Task<InstanceDTO> GetByIdDTO(int id)
        {
            return await _repository.GetByIdDTO(id);
        }

        public async Task<InstanceDTO> UpdateDTO(InstanceDTO i)
        {
            return await _repository.UpdateDTO(i);
        }

        public async Task DeleteDTO(int id)
        {
            await _repository.DeleteDTO(id);
        }

        public async Task<InstanceMatrixFacilityDTO[]> GetAllWeekMatrix(DateOnly date, int versionId)
        {
            return await _repository.GetAllWeekMatrix(date, versionId);
        }

        public async Task<MatrixLayout> GetMatrixLayout()
        {
            return await _repository.GetMatrixLayout();
        }
    }
}

