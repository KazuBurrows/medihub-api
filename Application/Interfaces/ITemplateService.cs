using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface ITemplateService
    {
        Task<TemplateMatrixFormatAgg> GetMatrixFormat(int facilityId);
        Task<TemplateDTO> GetByIdDTO(int id);
        Task<TemplateDTO> PutTemplateDTO(int id, int sessionId, int assetId, int week, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, bool isOpen, bool force, int versionId);
        Task<TemplateDTO> CreateTemplateDTO(int sessionId, int assetId, int week, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, bool isOpen, bool force, int versionId);
        Task Delete(int id);
        Task<string> ApplyTemplate(DateOnly date, int cycleWeek);
        Task<IEnumerable<TemplateDTO>> GetAllDTO();
        Task<IEnumerable<TemplateDTO>> GetAllDTOByWeek(int week, int versionId);

        Task<Template> GetById(int id);
        Task<int> Update(Template t);
        Task<int> Create(Template t);

        Task<MatrixLayout> GetMatrixLayout();
    }
}