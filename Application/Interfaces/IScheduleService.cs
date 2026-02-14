using MediHub.Domain.DTOs;
using MediHub.Domain.Models;

namespace MediHub.Application.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDTO>> GetMatrixByWeek(int year, int week);
        Task<IEnumerable<ScheduleDTO>> GetAllDTO(int year);
        Task<IEnumerable<ScheduleDTO>> GetMatrix(int year, int week, int? facility, int? asset);
        Task<MatrixFormatAgg> GetMatrixFormat(int facilityId);
        Task<InstanceDetailDTO> GetInstanceDetailDTO(int instance);
        Task<InstanceDetailDTO> PutInstanceDetailDTO(int id, int sessionId, int assetId, string startDatetime, string endDatetime, List<StaffDTO> staffs, bool force);
        Task<InstanceDetailDTO> CreateInstanceDetailDTO(int sessionId, int assetId, string startDatetime, string endDatetime, List<StaffDTO> staffs, bool force);
        Task<IEnumerable<ScheduleDTO>> GetList();
    }
}