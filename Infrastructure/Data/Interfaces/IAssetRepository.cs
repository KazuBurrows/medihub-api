using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IAssetRepository
    {
        Task<int> Create(Asset t);
        Task Delete(int id);
        Task<int> Update(Asset t);
        Task<IEnumerable<Asset>> GetAll();
        Task<Asset> GetById(int id);

        Task<IEnumerable<AssetDTO>> GetAllDTO();
        Task<AssetDTO> GetByIdDTO(int id);
    }
}
