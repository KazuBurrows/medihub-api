using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ITemplateService
    {
        Task<IEnumerable<TemplateMatrixDTO>> GetMatrix(int week, int? facility, int? theatre);
        Task<TemplateMatrixFormatAgg> GetMatrixFormat(int facilityId);
        Task<TemplateDetailDTO> GetTemplateDetailDTO(int id);
        Task<TemplateDetailDTO> PutTemplateDetailDTO(int id, int sessionId, int theatreId, int week, byte dayOfWeek, TimeSpan startTime, TimeSpan endTime, List<int> staffs);
        Task<TemplateDetailDTO> CreateTemplateDetailDTO(int sessionId, int theatreId, int week, byte dayOfWeek, TimeSpan startTime, TimeSpan endTime, List<int> staffs);
        Task<int> Delete(int id);
        Task ApplyTemplate(DateOnly date);
    }
}