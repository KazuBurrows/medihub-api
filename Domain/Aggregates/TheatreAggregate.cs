using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.DTOs
{
    public class TheatreAggregate
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [Column("facility_id")]
        public int? FacilityId { get; set; }
        [Column("scope_theatre_code")]
        public int? ScopeTheatreCode { get; set; }
        public int? Pediatric { get; set; }

        // junction table
        public List<int> EquipmentIds { get; set; } = new();
    }
}
