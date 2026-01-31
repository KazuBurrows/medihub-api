using MediHub.Domain.Models;

namespace MediHub.Domain.DTOs
{
    public class InstanceDetailDTO
    {
        public int Id { get; set; }

        public int? SessionId { get; set; }
        public string? SessionName { get; set; }
        public int? IsAcute { get; set; }
        public int? IsPediatric { get; set; }
        public string? AnaestheticType { get; set; }
        public string? SurgeonName { get; set; }
        public string? SpecialtyName { get; set; }
        public string? SubspecialtyName { get; set; }
        
        public string? FacilityName { get; set; }
        public int? TheatreId { get; set; }
        public string? TheatreName { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public Staff[]? Staffs { get; set; }
    }
}
