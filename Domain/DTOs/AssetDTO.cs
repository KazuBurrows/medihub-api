using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.DTOs
{
    public class AssetDTO
    {
        public int Id { get; set; }
        public string? AssetName { get; set; }

        [Column("scope_asset_code")]
        public int? ScopeAssetCode { get; set; }
        public int? Pediatric { get; set; }

        public string? FacilityName { get; set; }

        public string[]? EquipmentNames { get; set; }
    }
}
