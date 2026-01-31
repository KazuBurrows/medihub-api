using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDTO>> GetMatrixByWeek(int year, int week);
        Task<IEnumerable<ScheduleDTO>> GetAllDTO(int year);
        Task<IEnumerable<MatrixDTO>> GetMatrix(int year, int week, int? facility, int? theatre);
        Task<MatrixFormatAgg> GetMatrixFormat(int facilityId);
        Task<InstanceDetailDTO> GetInstanceDetailDTO(int instance);
        Task<InstanceDetailDTO> PutInstanceDetailDTO(int id, int sessionId, int theatreId, string startDatetime, string endDatetime, List<int> staffs, bool force);
        Task<InstanceDetailDTO> CreateInstanceDetailDTO(int sessionId, int theatreId, string startDatetime, string endDatetime, List<int> staffs, bool force);
        Task<IEnumerable<ListDTO>> GetList();
    }
}