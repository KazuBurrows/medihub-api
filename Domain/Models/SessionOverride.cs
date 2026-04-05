using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("session_override")]
    public class SessionOverride
    {
        [Key]
        [Column("SESSION_OVERRIDE_KEY")]
        public int Id { get; set; }

        [Column("SESSION_OVERRIDE_IS_ACUTE")]
        public bool? IsAcute { get; set; }

        [Column("SESSION_OVERRIDE_IS_PAEDIATRIC")]
        public bool? IsPaediatric { get; set; }

        [Column("SESSION_OVERRIDE_ANAESTHETIC_TYPE")]
        public string? AnaestheticType { get; set; }

        [Column("SESSION_OVERRIDE_SPECIALTY_KEY")]
        public int? SpecialtyId { get; set; }

        [Column("SESSION_OVERRIDE_SUBSPECIALTY_KEY")]
        public int? SubspecialtyId { get; set; }

        [Column("SESSION_OVERRIDE_SURGEON_KEY")]
        public int? SurgeonId { get; set; }

        [Column("SESSION_OVERRIDE_SURGEON_TYPE")]
        public string? SurgeonType { get; set; }
    }
}