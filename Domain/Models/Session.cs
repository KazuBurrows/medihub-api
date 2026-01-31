namespace MediHub.Domain.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? IsAcute { get; set; }
        public int? IsPediatric { get; set; }
        public string? AnaestheticType { get; set; }

        public int? SurgeonId { get; set; }
        public int? SpecialtyId { get; set; }
        public int? SubspecialtyId { get; set; }
    }
}