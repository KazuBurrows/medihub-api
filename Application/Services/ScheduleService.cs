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
    }
}
