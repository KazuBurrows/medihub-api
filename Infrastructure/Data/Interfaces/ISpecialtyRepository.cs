using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface ISpecialtyRepository
    {
        Task<int> Create(Specialty s);
        Task Delete(int id);
        Task<int> Update(Specialty s);
        Task<IEnumerable<Specialty>> GetAll();
        Task<Specialty> GetById(int id);
    }
}
