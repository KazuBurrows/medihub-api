using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IAnaestheticTypeService
    {
        Task<int> Create(AnaestheticType s);
        Task Delete(int id);
        Task<int> Update(AnaestheticType s);
        Task<IEnumerable<AnaestheticType>> GetAll();
        Task<AnaestheticType> GetById(int id);
    }
}