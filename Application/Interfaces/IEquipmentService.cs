using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IEquipmentService
    {
        Task<Equipment> Create(Equipment e);
        Task Delete(int id);
        Task<Equipment> Update(Equipment e);
        Task<IEnumerable<Equipment>> GetAll();
        Task<Equipment> GetById(int id);
    }
}