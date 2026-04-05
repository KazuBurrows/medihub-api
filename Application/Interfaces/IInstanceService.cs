using MediHub.Domain.DTOs;
using MediHub.Domain.Matrix;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IInstanceService
    {
        Task<Instance> Create(Instance i);
        Task Delete(int id);
        Task<Instance> Update(Instance i);
        Task<IEnumerable<Instance>> GetAll();
        Task<Instance> GetById(int id);
        Task<IEnumerable<InstanceDTO>> GetAllByStaffId(int staffId);

        Task<IEnumerable<InstanceDTO>> GetAllDTO();
        Task<IEnumerable<InstanceDTO>> GetAllDTOByDate(string startDate, string endDate);

        Task<InstanceDTO> CreateDTO(InstanceDTO i);
        Task<InstanceDTO> GetByIdDTO(int id);
        Task<InstanceDTO> UpdateDTO(InstanceDTO i);
        Task DeleteDTO(int id);

        Task<InstanceMatrixFacilityDTO[]> GetAllWeekMatrix(DateOnly date);

        Task<MatrixLayout> GetMatrixLayout();
    }
}