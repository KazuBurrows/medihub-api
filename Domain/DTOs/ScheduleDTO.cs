namespace MediHub.Domain.DTOs
{
    public class ScheduleDTO
    {
        public int Id { get; set; }

        public int? FacilityId { get; set; }
        public string? FacilityName { get; set; }
        public int? TheatreId { get; set; }
        public string? TheatreName { get; set; }
        public int? SessionId { get; set; }
        public string? SessionName { get; set; }

        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }
}
