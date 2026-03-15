using System.ComponentModel.DataAnnotations;

public class AssetInputDTO
{
    [Required] public string Code { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string TypeCode { get; set; }
    [Required] public string Location { get; set; }
    [Required] public int FacilityId { get; set; }
    [Required] public int DistrictOfService { get; set; }
    [Required] public int PrimarySpecialtyId { get; set; }
    [Required] public bool Paediatric { get; set; }
    [Required] public bool DedicatedAcute { get; set; }

}