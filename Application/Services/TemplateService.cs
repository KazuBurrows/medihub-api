
using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
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

        public async Task<TemplateDTO> GetByIdDTO(int id)
        {
            return await _repository.GetByIdDTO(id);
        }

        public async Task<TemplateDTO> CreateTemplateDTO(int sessionId, int assetId, int week, byte dayOfWeek, TimeSpan starTime, TimeSpan endTime, bool force)
        {
            return await _repository.CreateTemplateDTO(sessionId, assetId, week, dayOfWeek, starTime, endTime, force);
        }

        public async Task<TemplateDTO> PutTemplateDTO(int id, int sessionId, int assetId, int week, byte dayOfWeek, TimeSpan starTime, TimeSpan endTime, bool force)
        {
            return await _repository.PutTemplateDTO(id, sessionId, assetId, week, dayOfWeek, starTime, endTime, force);
        }

        public async Task<int> Delete(int id)
        {
            return await _repository.Delete(id);
        }

        public async Task<string> ApplyTemplate(DateOnly date)
        {
            return await _repository.ApplyTemplate(date);
        }

        public async Task<IEnumerable<TemplateDTO>> GetAllDTO()
        {
            return await _repository.GetAllDTO();
        }

        public async Task<Template> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public Task<int> Update(Template t)
        {
            return _repository.Update(t);
        }

        public Task<int> Create(Template t)
        {
            return _repository.Create(t);
        }
    }
}