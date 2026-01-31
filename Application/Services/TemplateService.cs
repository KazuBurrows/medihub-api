
using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Infrastructure.Data.Interfaces;
using MediHub.Infrastructure.Data.Utils;

namespace MediHub.Application.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _repository;

        public TemplateService(ITemplateRepository repository)
        {
            _repository = repository;
        }


        public async Task<IEnumerable<TemplateMatrixDTO>> GetMatrix(int week, int? facility, int? theatre)
        {
            return await _repository.GetMatrix(week, facility, theatre);
        }

        public async Task<TemplateMatrixFormatAgg> GetMatrixFormat(int facilityId)
        {
            return await _repository.GetMatrixFormat(facilityId);
        }

        public async Task<TemplateDetailDTO> GetTemplateDetailDTO(int id)
        {
            return await _repository.GetTemplateDetailDTO(id);
        }

        public async Task<TemplateDetailDTO> CreateTemplateDetailDTO(int sessionId, int theatreId, int week, byte dayOfWeek, TimeSpan starTime, TimeSpan endTime, List<int> staffs)
        {
            return await _repository.CreateTemplateDetailDTO(sessionId, theatreId, week, dayOfWeek, starTime, endTime, staffs);
        }

        public async Task<TemplateDetailDTO> PutTemplateDetailDTO(int id, int sessionId, int theatreId, int week, byte dayOfWeek, TimeSpan starTime, TimeSpan endTime, List<int> staffs)
        {
            return await _repository.PutTemplateDetailDTO(id, sessionId, theatreId, week, dayOfWeek, starTime, endTime, staffs);
        }

        public async Task<int> Delete(int id)
        {
            return await _repository.Delete(id);
        }

        public async Task ApplyTemplate(DateOnly date)
        {
            await _repository.ApplyTemplate(date);
        }
    }
}