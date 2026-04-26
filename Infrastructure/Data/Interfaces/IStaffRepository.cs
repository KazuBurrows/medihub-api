using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IStaffRepository
    {
        Task<Staff> Create(Staff s);
        Task Delete(int id);
        Task<Staff> Update(Staff s);
        Task<IEnumerable<Staff>> GetAll();
        Task<Staff> GetById(int id);
        Task<Staff> GetByEmail(string email);
        Task<IEnumerable<StaffDTO>> GetAllDTO();
        Task<StaffDTO> GetByIdDTO(int id);
    }
}
