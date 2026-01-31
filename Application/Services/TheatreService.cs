using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Application.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository _repository;

        public TheatreService(ITheatreRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Create(Theatre t)
        {
            return _repository.Create(t);
        }

        public Task<int> Delete(int id)
        {
            return _repository.Delete(id);
        }

        public Task<int> Update(Theatre t)
        {
            return _repository.Update(t);
        }

        public async Task<IEnumerable<Theatre>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Theatre> GetById(int id)
        {
            return await _repository.GetById(id);
        }


        public Task<int> CreateAgg(TheatreAggregate t)
        {
            return _repository.CreateAgg(t);
        }

        public Task<int> DeleteAgg(int id)
        {
            return _repository.DeleteAgg(id);
        }

        public Task<int> UpdateAgg(TheatreAggregate t)
        {
            return _repository.UpdateAgg(t);
        }

        public async Task<IEnumerable<TheatreAggregate>> GetAllAgg()
        {
            return await _repository.GetAllAgg();
        }

        public async Task<TheatreAggregate> GetByIdAgg(int id)
        {
            return await _repository.GetByIdAgg(id);
        }

        public async Task<IEnumerable<TheatreDTO>> GetAllDTO()
        {
            return await _repository.GetAllDTO();
        }

        public async Task<TheatreDTO> GetByIdDTO(int id)
        {
            return await _repository.GetByIdDTO(id);
        }
    }
}

