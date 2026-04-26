using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<Equipment> Create(Equipment e);
        Task Delete(int id);
        Task<Equipment> Update(Equipment e);
        Task<IEnumerable<Equipment>> GetAll();
        Task<Equipment> GetById(int id);
    }
}
