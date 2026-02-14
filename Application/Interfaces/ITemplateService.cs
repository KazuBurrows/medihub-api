using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ITemplateService
    {
        Task<IEnumerable<TemplateScheduleDTO>> GetMatrix(int week, int? facility, int? asset);
        Task<TemplateMatrixFormatAgg> GetMatrixFormat(int facilityId);
        Task<TemplateDetailDTO> GetTemplateDetailDTO(int id);
        Task<TemplateDetailDTO> PutTemplateDetailDTO(int id, int sessionId, int assetId, int week, byte dayOfWeek, TimeSpan startTime, TimeSpan endTime, bool force);
        Task<TemplateDetailDTO> CreateTemplateDetailDTO(int sessionId, int assetId, int week, byte dayOfWeek, TimeSpan startTime, TimeSpan endTime, bool force);
        Task<int> Delete(int id);
        Task<string> ApplyTemplate(DateOnly date, bool force);
        Task<IEnumerable<TemplateDetailDTO>> GetAllDTO();
    }
}