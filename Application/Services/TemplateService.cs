
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


        public async Task<IEnumerable<TemplateScheduleDTO>> GetMatrix(int week, int? facility, int? asset)
        {
            return await _repository.GetMatrix(week, facility, asset);
        }

        public async Task<TemplateMatrixFormatAgg> GetMatrixFormat(int facilityId)
        {
            return await _repository.GetMatrixFormat(facilityId);
        }

        public async Task<TemplateDetailDTO> GetTemplateDetailDTO(int id)
        {
            return await _repository.GetTemplateDetailDTO(id);
        }

        public async Task<TemplateDetailDTO> CreateTemplateDetailDTO(int sessionId, int assetId, int week, byte dayOfWeek, TimeSpan starTime, TimeSpan endTime, bool force)
        {
            return await _repository.CreateTemplateDetailDTO(sessionId, assetId, week, dayOfWeek, starTime, endTime, force);
        }

        public async Task<TemplateDetailDTO> PutTemplateDetailDTO(int id, int sessionId, int assetId, int week, byte dayOfWeek, TimeSpan starTime, TimeSpan endTime, bool force)
        {
            return await _repository.PutTemplateDetailDTO(id, sessionId, assetId, week, dayOfWeek, starTime, endTime, force);
        }

        public async Task<int> Delete(int id)
        {
            return await _repository.Delete(id);
        }

        public async Task<string> ApplyTemplate(DateOnly date, bool force)
        {
            return await _repository.ApplyTemplate(date, force);
        }

        public async Task<IEnumerable<TemplateDetailDTO>> GetAllDTO()
        {
            return await _repository.GetAllDTO();
        }
    }
}