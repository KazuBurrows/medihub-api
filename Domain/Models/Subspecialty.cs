using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("subspecialty")]
    public class Subspecialty
    {
        [Key]
        [Column("SUBSPECIALTY_KEY")]
        public int Id { get; set; }

        [Column("SUBSPECIALTY_NAME")]
        public string? Name { get; set; }
    }
}
