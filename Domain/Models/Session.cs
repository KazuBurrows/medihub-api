using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("session")]
    public class Session
    {
        [Key]
        [Column("SESSION_KEY")]
        public int Id { get; set; }

        [Column("SESSION_TITLE")]
        public string? Name { get; set; }

        [Column("SESSION_IS_ACUTE")]
        public bool? IsAcute { get; set; }

        [Column("SESSION_IS_PAEDIATRIC")]
        public bool? IsPediatric { get; set; }

        [Column("SESSION_ANAESTHETIC_TYPE_KEY")]
        public int? AnaestheticTypeId { get; set; }

        [Column("SESSION_SURGEON_KEY")]
        public int? SurgeonId { get; set; }

        [Column("SESSION_SURGEON_TYPE_KEY")]
        public int? SurgeonTypeId { get; set; }

        [Column("SESSION_SPECIALTY_KEY")]
        public int? SpecialtyId { get; set; }

        [Column("SESSION_SUBSPECIALTY_KEY")]
        public int? SubspecialtyId { get; set; }

    }

}