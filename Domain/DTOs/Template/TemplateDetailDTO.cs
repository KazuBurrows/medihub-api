using MediHub.Domain.Models;

namespace MediHub.Domain.DTOs
{
    public class TemplateDetailDTO
    {
        public int Id { get; set; }

        public int SessionId { get; set; }
        public string? SessionName { get; set; }
        public int? IsAcute { get; set; }
        public int? IsPediatric { get; set; }
        public string? AnaestheticType { get; set; }
        public string? SurgeonName { get; set; }
        public string? SpecialtyName { get; set; }
        public string? SubspecialtyName { get; set; }

        public string? FacilityName { get; set; }
        public int AssetId { get; set; }
        public string? AssetName { get; set; }

        /// <summary>
        /// Week number in the rotation (e.g., Week 1, Week 2)
        /// </summary>
        public int Week { get; set; }

        /// <summary>
        /// 0 = Sunday, 1 = Monday ... 6 = Saturday (depends how you store it)
        /// </summary>
        public byte DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsOpen { get; set; }

    }
}
