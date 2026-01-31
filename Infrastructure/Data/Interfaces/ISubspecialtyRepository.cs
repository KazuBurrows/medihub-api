using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface ISubspecialtyRepository
    {
        Task<int> Create(Subspecialty s);
        Task<int> Delete(int id);
        Task<int> Update(Subspecialty s);
        Task<IEnumerable<Subspecialty>> GetAll();
        Task<Subspecialty> GetById(int id);
    }
}
