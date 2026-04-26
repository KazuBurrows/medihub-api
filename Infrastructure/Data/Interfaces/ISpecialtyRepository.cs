using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface ISpecialtyRepository
    {
        Task<Specialty> Create(Specialty s);
        Task Delete(int id);
        Task<Specialty> Update(Specialty s);
        Task<IEnumerable<Specialty>> GetAll();
        Task<Specialty> GetById(int id);
        Task<IEnumerable<SpecialtyDTO>> GetAllDTO();
        Task<IEnumerable<Subspecialty>> GetSubspecialtiesBySpecialty(int id);
    }
}
