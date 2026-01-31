using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ITheatreService
    {
        Task<int> Create(Theatre t);
        Task<int> Delete(int id);
        Task<int> Update(Theatre t);
        Task<IEnumerable<Theatre>> GetAll();
        Task<Theatre> GetById(int id);

        // Theatre with junction table
        Task<int> CreateAgg(TheatreAggregate t);
        Task<int> DeleteAgg(int id);
        Task<int> UpdateAgg(TheatreAggregate t);
        Task<IEnumerable<TheatreAggregate>> GetAllAgg();
        Task<TheatreAggregate> GetByIdAgg(int id);

        Task<IEnumerable<TheatreDTO>> GetAllDTO();
        Task<TheatreDTO> GetByIdDTO(int id);

    }
}