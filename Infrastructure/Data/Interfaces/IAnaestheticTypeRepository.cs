using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IAnaestheticTypeRepository
    {
        Task<int> Create(AnaestheticType s);
        Task Delete(int id);
        Task<int> Update(AnaestheticType s);
        Task<IEnumerable<AnaestheticType>> GetAll();
        Task<AnaestheticType> GetById(int id);
    }
}
