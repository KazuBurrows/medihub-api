using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("surgeon_type")]
    public class SurgeonType
    {
        [Key]
        [Column("SURGEON_TYPE_KEY")]
        public int Id { get; set; }

        [Column("SURGEON_TYPE_CODE")]
        public string? Code { get; set; }

        [Column("SURGEON_TYPE_DESCRIPTION")]
        public string? Description { get; set; }
    }
}
