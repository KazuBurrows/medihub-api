using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IAssetService
    {
        Task<int> Create(Asset t);
        Task<int> Delete(int id);
        Task<int> Update(Asset t);
        Task<IEnumerable<Asset>> GetAll();
        Task<Asset> GetById(int id);

        // Asset with junction table
        Task<int> CreateAgg(AssetAggregate t);
        Task<int> DeleteAgg(int id);
        Task<int> UpdateAgg(AssetAggregate t);
        Task<IEnumerable<AssetAggregate>> GetAllAgg();
        Task<AssetAggregate> GetByIdAgg(int id);

        Task<IEnumerable<AssetDTO>> GetAllDTO();
        Task<AssetDTO> GetByIdDTO(int id);

    }
}