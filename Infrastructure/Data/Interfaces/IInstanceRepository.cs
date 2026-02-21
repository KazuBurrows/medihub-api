using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IInstanceRepository
    {
        Task<int> Create(Instance s);
        Task<int> Delete(int id);
        Task<int> Update(Instance s);
        Task<IEnumerable<Instance>> GetAll();
        Task<Instance> GetById(int id);
        Task<IEnumerable<ScheduleDTO>> GetAllByStaffId(int staffId);
        Task<IEnumerable<InstanceDTO>> GetAllDTO();

        Task<int> CreateDTO(InstanceDTO i);
        Task<InstanceDTO> GetByIdDTO(int id);
        Task<InstanceDTO> UpdateDTO(InstanceDTO i);
        Task<int> DeleteDTO(int id);
    }
}
