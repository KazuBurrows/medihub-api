using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("instance")]
    public class Instance
    {
        [Key]
        [Column("INSTANCE_KEY")]
        public int Id { get; set; }

        [Column("INSTANCE_ASSET_KEY")]
        public int? AssetId { get; set; }

        [Column("INSTANCE_SESSION_KEY")]
        public int? SessionId { get; set; }

        [Column("INSTANCE_START_DATETIME")]
        public DateTime? StartDatetime { get; set; }

        [Column("INSTANCE_END_DATETIME")]
        public DateTime? EndDatetime { get; set; }

        [Column("INSTANCE_IS_OPEN")]
        public bool? IsOpen { get; set; }
    }
}
