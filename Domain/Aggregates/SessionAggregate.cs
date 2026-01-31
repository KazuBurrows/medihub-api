namespace MediHub.Domain.DTOs
{
    public class SessionAggregate
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? IsAcute { get; set; }
        public int? IsPediatric { get; set; }
        public string? AnaestheticType { get; set; }

        // junction table
        public List<int>? StaffIds { get; set; }
        public List<int>? SubspecialtyIds { get; set; }
    }
}
