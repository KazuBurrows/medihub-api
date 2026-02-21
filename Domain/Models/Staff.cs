using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("staff")]
    public class Staff
    {
        [Key]
        [Column("STAFF_KEY")]
        public int Id { get; set; }

        [Column("STAFF_ID")]
        public int? StaffId { get; set; }

        [Column("STAFF_NAME")]
        public string? Name { get; set; }

        [Column("STAFF_EMAIL")]
        public string? Email { get; set; }

        [Column("STAFF_SPECIALTY_KEY")]
        public int? SpecialtyId { get; set; }
    }
}
