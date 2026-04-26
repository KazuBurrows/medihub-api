using System.ComponentModel.DataAnnotations;

public class TemplateInputDTO
{
    [Required] public int SessionId { get; set; }
    [Required] public int AssetId { get; set; }
    [Required] public int CycleWeek { get; set; }
    [Required] public int CycleDay { get; set; }
    [Required] public TimeSpan StartTime { get; set; }
    [Required] public TimeSpan EndTime { get; set; }
    public bool IsOpen { get; set; }
    public bool Force { get; set; } = false;
    [Required] public int VersionId { get; set; }
}