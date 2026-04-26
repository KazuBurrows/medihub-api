using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    [Table("instance_template")]
    public class Template
    {
        [Key]
        [Column("INSTANCE_TEMPLATE_KEY")]
        public int Id { get; set; }

        [Column("INSTANCE_TEMPLATE_SESSION_KEY")]
        public int? SessionId { get; set; }

        [Column("INSTANCE_TEMPLATE_ASSET_KEY")]
        public int? AssetId { get; set; }

        [Column("INSTANCE_TEMPLATE_CYCLE_WEEK")]
        public int? CycleWeek { get; set; }

        [Column("INSTANCE_TEMPLATE_CYCLE_DAY")]
        public int? CycleDay { get; set; }

        [Column("INSTANCE_TEMPLATE_IS_OPEN")]
        public bool? IsOpen { get; set; }

        [Column("INSTANCE_TEMPLATE_START_TIME")]
        public TimeSpan? StartTime { get; set; }

        [Column("INSTANCE_TEMPLATE_END_TIME")]
        public TimeSpan? EndTime { get; set; }

        [Column("INSTANCE_TEMPLATE_LAST_UPDATED_DATETIME")]
        public DateTime? LastUpdatedDatetime { get; set; }

        [Column("INSTANCE_TEMPLATE_LAST_UPDATED_USER_KEY")]
        public int? LastUpdatedByUserId { get; set; }

        [Column("INSTANCE_TEMPLATE_VERSION_KEY")]
        public int? VersionId { get; set; }
    }
}