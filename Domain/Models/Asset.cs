using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("asset")]
    public class Asset
    {
        [Key]
        [Column("ASSET_KEY")]
        public int Id { get; set; }

        [Column("ASSET_CODE")]
        public string? Code { get; set; }

        [Column("ASSET_DESCRIPTION")]
        public string? Description { get; set; }

        [Column("ASSET_TYPE_CODE")]
        public string? TypeCode { get; set; }

        [Column("ASSET_LOCATION")]
        public string? Location { get; set; }

        [Column("ASSET_FACILITY_KEY")]
        public int? FacilityId { get; set; }

        [Column("ASSET_DISTRICT_OF_SERVICE")]
        public int? DistrictOfService { get; set; }

        [Column("ASSET_PRIMARY_SPECIALTY_KEY")]
        public int? PrimarySpecialtyId { get; set; }

        [Column("ASSET_PAEDIATRIC")]
        public bool? Paediatric { get; set; }

        [Column("ASSET_DEDICATED_ACUTE")]
        public bool? DedicatedAcute { get; set; }
    }

}
