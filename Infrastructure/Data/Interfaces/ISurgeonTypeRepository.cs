using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface ISurgeonTypeRepository
    {
        Task<SurgeonType> Create(SurgeonType s);
        Task Delete(int id);
        Task<SurgeonType> Update(SurgeonType s);
        Task<IEnumerable<SurgeonType>> GetAll();
        Task<SurgeonType> GetById(int id);
    }
}
