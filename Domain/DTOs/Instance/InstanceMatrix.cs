using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MediHub.Domain.Models;

namespace MediHub.Domain.Matrix
{
    public class InstanceMatrixFacilityDTO
    {
        [Column("FACILITY_KEY")]
        public int? FacilityId { get; set; }

        [Column("FACILITY_NAME")]
        public string? FacilityName { get; set; } = "";

        public List<InstanceMatrixAssetDTO> Assets { get; set; } = new();
    }

    public class InstanceMatrixAssetDTO
    {
        [Column("ASSET_KEY")]
        public int? AssetId { get; set; }

        [Column("ASSET_DESCRIPTION")]
        public string? AssetDescription { get; set; } = "";

        public List<InstanceMatrixDayDTO> Days { get; set; } = new();
    }

    public class InstanceMatrixDayDTO
    {
        public DateTime Date { get; set; }

        public string DayName { get; set; } = ""; // Mon, Tue, etc

        public List<InstanceMatrixCellDTO> Instance { get; set; } = new();
    }

    public class InstanceMatrixCellDTO
    {
        [Column("INSTANCE_KEY")]
        public int Id { get; set; }


        [Column("FACILITY_KEY")]
        public int? FacilityId { get; set; }

        [Column("FACILITY_NAME")]
        public string? FacilityName { get; set; } = "";

        [Column("ASSET_KEY")]
        public int? AssetId { get; set; }

        [Column("ASSET_DESCRIPTION")]
        public string? AssetDescription { get; set; } = "";


        [Column("INSTANCE_VERSION_KEY")]
        public int? VersionId { get; set; }
        public string? VersionName { get; set; } = "";
        public string? VersionDescription { get; set; } = "";
        public bool? VersionIsActive { get; set; } = false;


        [Column("SESSION_KEY")]
        public int? SessionId { get; set; }

        [Column("SESSION_TITLE")]
        public string? SessionTitle { get; set; }

        [Column("SESSION_IS_ACUTE")]
        public bool? IsAcute { get; set; }

        [Column("SESSION_IS_PAEDIATRIC")]
        public bool? IsPediatric { get; set; }

        [Column("SESSION_ANAESTHETIC_TYPE_KEY")]
        public int? AnaestheticTypeId { get; set; }
        public string? AnaestheticTypeCode { get; set; }
        public string? AnaestheticTypeDescription { get; set; }

        [Column("SESSION_SURGEON_KEY")]
        public int? SurgeonId { get; set; }

        [Column("SESSION_SURGEON_TYPE_KEY")]
        public int? SurgeonTypeId { get; set; }

        [Column("SESSION_SURGEON_TYPE_CODE")]
        public string? SurgeonTypeCode { get; set; }

        [Column("SESSION_SURGEON_TYPE_DESCRIPTION")]
        public string? SurgeonTypeDescription { get; set; }

        [Column("STAFF_NAME")]
        public string? SurgeonName { get; set; }



        [Column("INSTANCE_START_DATETIME")]
        public DateTime? StartDateTime { get; set; }

        [Column("INSTANCE_END_DATETIME")]
        public DateTime? EndDateTime { get; set; }


        [Column("INSTANCE_IS_OPEN")]
        public bool? IsOpen { get; set; }

        public int? StaffCount { get; set; }

        public int? SessionOverrideCount { get; set; }
    }


}
