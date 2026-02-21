using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IStaffRepository
    {
        Task<int> Create(Staff s);
        Task<int> Delete(int id);
        Task<int> Update(Staff s);
        Task<IEnumerable<Staff>> GetAll();
        Task<Staff> GetById(int id);
        Task<Staff> GetByEmail(string email);
        Task<IEnumerable<StaffDTO>> GetAllDTO();
        Task<StaffDTO> GetByIdDTO(int id);
    }
}
