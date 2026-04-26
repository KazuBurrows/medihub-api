using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ISurgeonTypeService
    {
        Task<SurgeonType> Create(SurgeonType s);
        Task Delete(int id);
        Task<SurgeonType> Update(SurgeonType s);
        Task<IEnumerable<SurgeonType>> GetAll();
        Task<SurgeonType> GetById(int id);
    }
}