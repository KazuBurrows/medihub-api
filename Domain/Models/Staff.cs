using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    public class Staff
    {
        public int Id { get; set; }
        [Column("first_name")]
        public string? FirstName { get; set; }
        [Column("last_name")]
        public string? LastName { get; set; }
        public string? Code { get; set; }
        public string? Email { get; set; }
    }
}
