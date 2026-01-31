using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.DTOs
{
    public class StaffAggregate
    {
        public int Id { get; set; }
        [Column("first_name")]
        public string? FirstName { get; set; }
        [Column("last_name")]
        public string? LastName { get; set; }
        public string? Code { get; set; }
        public string? Email { get; set; }

        // junction table
        public List<int>? SessionIds { get; set; }
        public List<int>? SubspecialtyIds { get; set; }
    }
}
