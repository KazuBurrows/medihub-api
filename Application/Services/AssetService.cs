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

        public Task<Asset> Create(Asset t)
        {
            return _repository.Create(t);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }

        public Task<Asset> Update(Asset t)
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

