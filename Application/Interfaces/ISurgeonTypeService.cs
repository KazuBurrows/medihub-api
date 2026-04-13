using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ISurgeonTypeService
    {
        Task<int> Create(SurgeonType s);
        Task Delete(int id);
        Task<int> Update(SurgeonType s);
        Task<IEnumerable<SurgeonType>> GetAll();
        Task<SurgeonType> GetById(int id);
    }
}