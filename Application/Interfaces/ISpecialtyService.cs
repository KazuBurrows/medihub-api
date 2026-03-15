using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ISpecialtyService
    {
        Task<int> Create(Specialty s);
        Task Delete(int id);
        Task<int> Update(Specialty s);
        Task<IEnumerable<Specialty>> GetAll();
        Task<Specialty> GetById(int id);
    }
}