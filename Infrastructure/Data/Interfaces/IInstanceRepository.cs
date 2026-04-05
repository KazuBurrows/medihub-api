using MediHub.Domain.DTOs;
using MediHub.Domain.Matrix;
using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IInstanceRepository
    {
        Task<Instance> Create(Instance s);
        Task Delete(int id);
        Task<Instance> Update(Instance s);
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
