using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<ScheduleDTO>> GetMatrixByWeek(DateTime monday, DateTime sunday);
        Task<IEnumerable<ScheduleDTO>> GetAllDTO(DateTime startDateTime, DateTime endDateTime);
        Task<IEnumerable<MatrixDTO>> GetMatrix(DateTime monday, DateTime sunday, int? facility, int? theatre);
        Task<MatrixFormatAgg> GetMatrixFormat(int facilityId);
        Task<InstanceDetailDTO> GetInstanceDetailDTO(int instance);
        Task<InstanceDetailDTO> PutInstanceDetailDTO(int id, int sessionId, int theatreId, string startDatetime, string endDatetime, List<int> staffs, bool force);
        Task<InstanceDetailDTO> CreateInstanceDetailDTO(int sessionId, int theatreId, string startDatetime, string endDatetime, List<int> staffs, bool force);
        Task<IEnumerable<ListDTO>> GetList();
    }
}
