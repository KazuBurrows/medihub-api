using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IFacilityRepository
    {
        Task<Facility> Create(Facility f);
        Task Delete(int id);
        Task<Facility> Update(Facility f);
        Task<IEnumerable<Facility>> GetAll();
        Task<Facility> GetById(int id);
    }
}
