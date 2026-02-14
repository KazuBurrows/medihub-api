using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IInstanceService
    {
        Task<int> Create(Instance i);
        Task<int> Delete(int id);
        Task<int> Update(Instance i);
        Task<IEnumerable<Instance>> GetAll();
        Task<Instance> GetById(int id);
        Task<IEnumerable<ScheduleDTO>> GetAllByStaffId(int staffId);
    }
}