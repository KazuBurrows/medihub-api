using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [Column("facility_id")]
        public int? FacilityId { get; set; }
        [Column("scope_asset_code")]
        public int? ScopeAssetCode { get; set; }
        public int? Pediatric { get; set; }
    }
}
