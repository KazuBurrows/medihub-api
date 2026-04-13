namespace MediHub.Domain.DTOs
{
    public class TemplateScheduleDTO
    {
        public int Id { get; set; }

        public string? FacilityName { get; set; }
        public string? AssetName { get; set; }
        public string? SessionName { get; set; }

        public int? IsAcute { get; set; }
        public int? IsPediatric { get; set; }
        public int? AnaestheticTypeId { get; set; }
        public string? AnaestheticTypeCode { get; set; }
        public string? AnaestheticTypeDescription { get; set; }
        public string? SurgeonName { get; set; }
        public int? StaffCount { get; set; }
        
        public int Week { get; set; }
        public byte DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsOpen { get; set; }
    }
}
