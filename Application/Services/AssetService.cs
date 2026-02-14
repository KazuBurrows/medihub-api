using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _repository;

        public AssetService(IAssetRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(Asset t)
        {
            return _repository.Create(t);
        }

        public Task<int> Delete(int id)
        {
            return _repository.Delete(id);
        }

        public Task<int> Update(Asset t)
        {
            return _repository.Update(t);
        }

        public async Task<IEnumerable<Asset>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Asset> GetById(int id)
        {
            return await _repository.GetById(id);
        }


        public Task<int> CreateAgg(AssetAggregate t)
        {
            return _repository.CreateAgg(t);
        }

        public Task<int> DeleteAgg(int id)
        {
            return _repository.DeleteAgg(id);
        }

        public Task<int> UpdateAgg(AssetAggregate t)
        {
            return _repository.UpdateAgg(t);
        }

        public async Task<IEnumerable<AssetAggregate>> GetAllAgg()
        {
            return await _repository.GetAllAgg();
        }

        public async Task<AssetAggregate> GetByIdAgg(int id)
        {
            return await _repository.GetByIdAgg(id);
        }

        public async Task<IEnumerable<AssetDTO>> GetAllDTO()
        {
            return await _repository.GetAllDTO();
        }

        public async Task<AssetDTO> GetByIdDTO(int id)
        {
            return await _repository.GetByIdDTO(id);
        }
    }
}

