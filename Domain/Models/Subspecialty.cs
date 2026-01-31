using System.ComponentModel.DataAnnotations.Schema;

namespace MediHub.Domain.Models
{
    public class Subspecialty
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}
