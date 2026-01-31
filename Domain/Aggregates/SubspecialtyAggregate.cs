using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.DTOs
{
    public class SubspecialtyAggregate
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }

        // junction table
        public List<int>? SessionIds { get; set; }
        public List<int>? StaffIds { get; set; }
    }
}
