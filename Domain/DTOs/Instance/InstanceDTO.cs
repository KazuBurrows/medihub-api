using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MediHub.Domain.Models;

namespace MediHub.Domain.DTOs
{
    public class InstanceDTO
    {
        [Key]
        [Column("INSTANCE_KEY")]
        public int Id { get; set; }



        [Column("INSTANCE_SESSION_KEY")]
        public int? SessionId { get; set; }

        [Column("SESSION_TITLE")]
        public string? SessionTitle { get; set; }

        [Column("SESSION_IS_ACUTE")]
        public bool? SessionIsAcute { get; set; }

        [Column("SESSION_IS_PAEDIATRIC")]
        public bool? SessionIsPaediatric { get; set; }

        [Column("SESSION_ANAESTHETIC_TYPE")]
        public string? AnaestheticType { get; set; }

        [Column("SESSION_SURGEON_KEY")]
        public int? SurgeonId { get; set; }
        public string? SurgeonName { get; set; }

        [Column("SESSION_SPECIALTY_KEY")]
        public int? SpecialtyId { get; set; }
        public string? SpecialtyCode { get; set; }

        [Column("SESSION_SUBSPECIALTY_KEY")]
        public int? SubspecialtyId { get; set; }
        public string? SubspecialtyName { get; set; }
        


        [Column("INSTANCE_ASSET_KEY")]
        public int? AssetId { get; set; }

        [Column("ASSET_CODE")]
        public string? AssetCode { get; set; }

        [Column("ASSET_LOCATION")]
        public string? AssetLocation { get; set; }

        [Column("ASSET_FACILITY_KEY")]
        public int? FacilityId { get; set; }
        public string? FacilityName { get; set; }

        public Equipment[]? Equipments { get; set; }



        [Column("INSTANCE_START_DATETIME")]
        public DateTime? StartDatetime { get; set; }

        [Column("INSTANCE_END_DATETIME")]
        public DateTime? EndDatetime { get; set; }

        [Column("INSTANCE_IS_OPEN")]
        public bool? IsOpen { get; set; }



        [Column("INSTANCE_LAST_UPDATED_DATETIME")]
        public DateTime? LastUpdatedDatetime { get; set; }

        [Column("INSTANCE_LAST_UPDATED_USER_KEY")]
        public int? LastUpdatedByUserId { get; set; }

        [Column("STAFF_ID")]
        public int? LastUpdatedByUserCode { get; set; }

        [Column("STAFF_NAME")]
        public string? LastUpdatedByUserName { get; set; }



        public List<StaffDTO>? Staffs { get; set; }

    }
}
