using MediHub.Domain.Models;
using Version = MediHub.Domain.Models.Version;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IVersionRepository
    {
        Task<Version> Create(Version v);
        Task Delete(int id);
        Task<Version> Update(Version v);
        Task<IEnumerable<Version>> GetAll();
        Task<Version> GetById(int id);
    }
}
