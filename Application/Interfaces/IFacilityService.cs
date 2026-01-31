using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IFacilityService
    {
        Task<int> Create(Facility f);
        Task<int> Delete(int id);
        Task<int> Update(Facility f);
        Task<IEnumerable<Facility>> GetAll();
        Task<Facility> GetById(int id);
    }
}