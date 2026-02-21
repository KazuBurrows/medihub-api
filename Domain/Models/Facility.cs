using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("facility")]
    public class Facility
    {
        [Key]
        [Column("FACILITY_KEY")]
        public int Id { get; set; }

        [Column("FACILITY_CODE")]
        public int? Code { get; set; }

        [Column("FACILITY_NAME")]
        public string? Name { get; set; }

        [Column("FACILITY_TYPE_CODE")]
        public int? TypeCode { get; set; }

        [Column("FACILITY_TYPE_DESCRIPTION")]
        public string? TypeDescription { get; set; }

        [Column("FACILITY_DHB_CODE")]
        public int? DhbCode { get; set; }

        [Column("FACILITY_DHB_NAME")]
        public string? DhbName { get; set; }
    }
}
