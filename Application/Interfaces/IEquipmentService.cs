using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IEquipmentService
    {
        Task<int> Create(Equipment e);
        Task Delete(int id);
        Task<int> Update(Equipment e);
        Task<IEnumerable<Equipment>> GetAll();
        Task<Equipment> GetById(int id);
    }
}