using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Infrastructure.Data.Interfaces
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<ScheduleDTO>> GetMatrixByWeek(DateTime monday, DateTime sunday);
        Task<IEnumerable<ScheduleDTO>> GetAllDTO(DateTime startDateTime, DateTime endDateTime);
        Task<IEnumerable<ScheduleDTO>> GetMatrix(DateTime monday, DateTime sunday, int? facility, int? asset);
        Task<MatrixFormatAgg> GetMatrixFormat(int facilityId);
        Task<InstanceDetailDTO> GetInstanceDetailDTO(int instance);
        Task<InstanceDetailDTO> PutInstanceDetailDTO(int id, int sessionId, int assetId, string startDatetime, string endDatetime, List<StaffDTO> staffs, bool force);
        Task<InstanceDetailDTO> CreateInstanceDetailDTO(int sessionId, int assetId, string startDatetime, string endDatetime, List<StaffDTO> staffs, bool force);
        Task<IEnumerable<ScheduleDTO>> GetList();
    }
}
