using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("specialty")]
    public class Specialty
    {
        [Key]
        [Column("SPECIALTY_KEY")]
        public int Id { get; set; }

        [Column("SPECIALTY_CODE")]
        public string? Code { get; set; }

        [Column("SPECIALTY_DESCRIPTION")]
        public string? Description { get; set; }

        [Column("SPECIALTY_IS_VISIBLE")]
        public bool? IsVisible { get; set; }
    }
}
