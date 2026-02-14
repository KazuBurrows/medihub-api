using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    public class StaffDTO
    {
        public int? Id { get; set; }
        [Column("first_name")]
        public string? FirstName { get; set; }
        [Column("last_name")]
        public string? LastName { get; set; }
        public string? Code { get; set; }
        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
