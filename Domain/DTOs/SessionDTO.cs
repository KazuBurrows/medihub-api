using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MediHub.Domain.Models;

namespace MediHub.Domain.DTOs
{
    public class SessionDTO
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
        public string? AnaestheticTypeCode { get; set; }
        public string? AnaestheticTypeDescription { get; set; }

        [Column("SESSION_SURGEON_KEY")]
        public int? SurgeonId { get; set; }

        [Column("SESSION_SURGEON_TYPE_KEY")]
        public int? SurgeonTypeId { get; set; }
        public string? SurgeonTypeCode { get; set; }
        public string? SurgeonTypeDescription { get; set; }

        public string? SurgeonName { get; set; }

        [Column("SESSION_SPECIALTY_KEY")]
        public int? SpecialtyId { get; set; }

        public string? SpecialtyCode { get; set; }

        public string? SpecialtyDescription { get; set; }

        [Column("SESSION_SUBSPECIALTY_KEY")]
        public int? SubspecialtyId { get; set; }
        
        public string? SubspecialtyName { get; set; }

    }
}
