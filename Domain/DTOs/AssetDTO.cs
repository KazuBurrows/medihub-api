using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.DTOs
{
    public class AssetDTO
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



        [Column("FACILITY_KEY")]
        public int? FacilityId { get; set; }

        [Column("FACILITY_CODE")]
        public int? FacilityCode { get; set; }

        [Column("FACILITY_NAME")]
        public string? FacilityName { get; set; }

        [Column("FACILITY_TYPE_CODE")]
        public int? FacilityTypeCode { get; set; }

        [Column("FACILITY_TYPE_DESCRIPTION")]
        public string? TypeDescription { get; set; }

        [Column("FACILITY_DHB_CODE")]
        public int? DhbCode { get; set; }

        [Column("FACILITY_DHB_NAME")]
        public string? DhbName { get; set; }



        [Column("ASSET_DISTRICT_OF_SERVICE")]
        public int? DistrictOfService { get; set; }



        [Column("ASSET_PRIMARY_SPECIALTY_KEY")]
        public int? PrimarySpecialtyId { get; set; }

        [Column("SPECIALTY_CODE")]
        public string? SpecialtyCode { get; set; }

        [Column("SPECIALTY_DESCRIPTION")]
        public string? SpecialtyDescription { get; set; }



        [Column("ASSET_PAEDIATRIC")]
        public bool? Paediatric { get; set; }

        [Column("ASSET_DEDICATED_ACUTE")]
        public bool? DedicatedAcute { get; set; }


        public string[]? EquipmentNames { get; set; }

    }
}
