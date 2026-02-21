using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("equipment")]
    public class Equipment
    {
        [Key]
        [Column("EQUIPMENT_KEY")]
        public int Id { get; set; }

        [Column("EQUIPMENT_NAME")]
        public string? Name { get; set; }
    }
}
