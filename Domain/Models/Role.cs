using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("role")]
    public class Role
    {
        [Key]
        [Column("ROLE_KEY")]
        public int Id { get; set; }

        [Column("ROLE_NAME")]
        public string? Name { get; set; }
    }
}
