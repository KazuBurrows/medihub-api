using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface ISurgeonTypeRepository
    {
        Task<int> Create(SurgeonType s);
        Task Delete(int id);
        Task<int> Update(SurgeonType s);
        Task<IEnumerable<SurgeonType>> GetAll();
        Task<SurgeonType> GetById(int id);
    }
}
