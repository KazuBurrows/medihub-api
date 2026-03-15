using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ISubspecialtyService
    {
        Task<int> Create(Subspecialty s);
        Task Delete(int id);
        Task<int> Update(Subspecialty s);
        Task<IEnumerable<Subspecialty>> GetAll();
        Task<Subspecialty> GetById(int id);
    }
}