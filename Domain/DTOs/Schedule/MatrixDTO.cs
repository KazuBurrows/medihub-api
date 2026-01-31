namespace MediHub.Domain.DTOs
{
    public class MatrixDTO
    {
        public int Id { get; set; }
        public string? FacilityName { get; set; }
        public string? TheatreName { get; set; }
        public string? SessionName { get; set; }
        public int? IsAcute { get; set; }
        public int? IsPediatric { get; set; }
        public string? AnaestheticType { get; set; }
        public string? SurgeonName { get; set; }
        public int? StaffCount { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }
}
