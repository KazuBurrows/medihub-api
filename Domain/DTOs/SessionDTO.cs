using MediHub.Domain.Models;

namespace MediHub.Domain.DTOs
{
    public class SessionDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? IsAcute { get; set; }
        public int? IsPediatric { get; set; }
        public string? AnaestheticType { get; set; }
        public string? SurgeonName { get; set; }
        public string? SpecialtyName { get; set; }
        public string? SubspecialtyName { get; set; }
        public int? InstanceCount { get ; set; }
    }
}
