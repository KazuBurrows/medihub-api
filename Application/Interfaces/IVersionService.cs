using MediHub.Domain.Models;
using Version = MediHub.Domain.Models.Version;

namespace MediHub.Application.Interfaces
{
    public interface IVersionService
    {
        Task<Version> Create(Version v);
        Task Delete(int id);
        Task<Version> Update(Version v);
        Task<IEnumerable<Version>> GetAll();
        Task<Version> GetById(int id);
    }
}