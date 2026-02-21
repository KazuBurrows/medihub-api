using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ITemplateService
    {
        Task<IEnumerable<TemplateScheduleDTO>> GetMatrix(int week, int? facility, int? asset);
        Task<TemplateMatrixFormatAgg> GetMatrixFormat(int facilityId);
        Task<TemplateDTO> GetByIdDTO(int id);
        Task<TemplateDTO> PutTemplateDTO(int id, int sessionId, int assetId, int week, byte dayOfWeek, TimeSpan startTime, TimeSpan endTime, bool force);
        Task<TemplateDTO> CreateTemplateDTO(int sessionId, int assetId, int week, byte dayOfWeek, TimeSpan startTime, TimeSpan endTime, bool force);
        Task<int> Delete(int id);
        // Task<string> ApplyTemplate(DateOnly date, bool force);
        Task<IEnumerable<TemplateDTO>> GetAllDTO();

        Task<Template> GetById(int id);
        Task<int> Update(Template t);
        Task<int> Create(Template t);
    }
}