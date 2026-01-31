using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;
using MediHub.Infrastructure.Data.Utils;

namespace MediHub.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _repository;

        public ScheduleService(IScheduleRepository repository)
        {
            _repository = repository;
        }


        public async Task<IEnumerable<ScheduleDTO>> GetMatrixByWeek(int year, int week)
        {
            var (monday, sunday) = WeekHelper.GetWeekDates(year, week);
            return await _repository.GetMatrixByWeek(monday, sunday);
        }

        public async Task<IEnumerable<ScheduleDTO>> GetAllDTO(int year)
        {
            var start_datetime = new DateTime(year, 1, 1);
            var end_datetime   = new DateTime(year, 12, 31, 23, 59, 59);

            return await _repository.GetAllDTO(start_datetime, end_datetime);
        }

        public async Task<IEnumerable<MatrixDTO>> GetMatrix(int year, int week, int? facility, int? theatre)
        {
            var (monday, sunday) = WeekHelper.GetWeekDates(year, week);
            return await _repository.GetMatrix(monday, sunday, facility, theatre);
        }

        public async Task<MatrixFormatAgg> GetMatrixFormat(int facilityId)
        {
            return await _repository.GetMatrixFormat(facilityId);
        }

        public async Task<InstanceDetailDTO> GetInstanceDetailDTO(int instance)
        {
            return await _repository.GetInstanceDetailDTO(instance);
        }

        public async Task<InstanceDetailDTO> PutInstanceDetailDTO(int id, int sessionId, int theatreId, string startDatetime, string endDatetime, List<int> staffs, bool force)
        {
            return await _repository.PutInstanceDetailDTO(id, sessionId, theatreId, startDatetime, endDatetime, staffs, force);
        }

        public async Task<InstanceDetailDTO> CreateInstanceDetailDTO(int sessionId, int theatreId, string startDatetime, string endDatetime, List<int> staffs, bool force)
        {
            return await _repository.CreateInstanceDetailDTO(sessionId, theatreId, startDatetime, endDatetime, staffs, force);
        }

        public async Task<IEnumerable<ListDTO>> GetList()
        {
            return await _repository.GetList();
        }
    }
}
