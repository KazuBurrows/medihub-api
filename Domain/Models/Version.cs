using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("version")]
    public class Version
    {
        [Key]
        [Column("VERSION_KEY")]
        public int Id { get; set; }

        [Column("VERSION_NAME")]
        public string? Name { get; set; }

        [Column("VERSION_DESCRIPTION")]
        public string? Description { get; set; }

        [Column("VERSION_IS_ACTIVE")]
        public bool? IsActive { get; set; }
    }
}