using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IStaffService
    {
        Task<int> Create(Staff s);
        Task<int> Delete(int id);
        Task<int> Update(Staff s);
        Task<IEnumerable<Staff>> GetAll();
        Task<Staff> GetById(int id);
        Task<Staff> GetByEmail(string email);
    }
}