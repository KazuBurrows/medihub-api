namespace MediHub.Domain.Models
{
    public class Instance
    {
        public int Id { get; set; }
        public int? AssetId { get; set; }
        public int? SessionId { get; set; }
        public DateTime? StartDatetime { get; set; }
        public DateTime? EndDatetime { get; set; }
    }
}
