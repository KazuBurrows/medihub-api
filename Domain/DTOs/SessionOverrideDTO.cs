using System;

namespace MediHub.Domain.DTOs
{
    public class SessionOverrideDTO
    {
        public int Id { get; set; }

        public bool? IsAcute { get; set; }

        public bool? IsPaediatric { get; set; }

        public string? AnaestheticType { get; set; }

        public int? SurgeonId { get; set; }

        public string? SurgeonName { get; set; }

        public string? SurgeonType { get; set; }

        public int? SpecialtyId { get; set; }

        public string? SpecialtyCode { get; set; }

        public string? SpecialtyDescription { get; set; }

        public int? SubspecialtyId { get; set; }

        public string? SubspecialtyName { get; set; }
    }
}