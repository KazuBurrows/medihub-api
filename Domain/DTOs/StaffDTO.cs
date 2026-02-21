using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    public class StaffDTO
    {
        [Key]
        [Column("STAFF_KEY")]
        public int Id { get; set; }

        [Column("STAFF_ID")]
        public int? StaffId { get; set; }

        [Column("STAFF_NAME")]
        public string? Name { get; set; }

        [Column("STAFF_EMAIL")]
        public string? Email { get; set; }



        [Column("STAFF_SPECIALTY_KEY")]
        public int? SpecialtyId { get; set; }

        [Column("SPECIALTY_CODE")]
        public string? SpecialtyCode { get; set; }

        [Column("SPECIALTY_DESCRIPTION")]
        public string? SpecialtyDescription { get; set; }


        public Subspecialty[]? Subspecialties { get; set; }


        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
