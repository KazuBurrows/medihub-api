using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.DTOs
{
    public class TheatreDTO
    {
        public int Id { get; set; }
        public string? TheatreName { get; set; }

        [Column("scope_theatre_code")]
        public int? ScopeTheatreCode { get; set; }
        public int? Pediatric { get; set; }

        public string? FacilityName { get; set; }

        public string[]? EquipmentNames { get; set; }
    }
}
