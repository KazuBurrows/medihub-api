using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("anaesthetic_type")]
    public class AnaestheticType
    {
        [Key]
        [Column("ANESTHETIC_TYPE_KEY")]
        public int Id { get; set; }

        [Column("ANESTHETIC_TYPE_CODE")]
        public string? Code { get; set; }

        [Column("ANESTHETIC_TYPE_DESCRIPTION")]
        public string? Description { get; set; }
    }
}
