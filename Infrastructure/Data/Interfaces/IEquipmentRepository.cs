using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<int> Create(Equipment e);
        Task Delete(int id);
        Task<int> Update(Equipment e);
        Task<IEnumerable<Equipment>> GetAll();
        Task<Equipment> GetById(int id);
    }
}
