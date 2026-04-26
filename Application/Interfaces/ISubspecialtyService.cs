using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ISubspecialtyService
    {
        Task<Subspecialty> Create(Subspecialty s);
        Task Delete(int id);
        Task<Subspecialty> Update(Subspecialty s);
        Task<IEnumerable<Subspecialty>> GetAll();
        Task<Subspecialty> GetById(int id);
    }
}