
namespace MediHub.Domain.Models
{
    public class MatrixLayout
    {
        public string[] Grouping { get; set; } = new string[0];
        public List<MatrixNode> Groups { get; set; } = new();
    }

    public class MatrixNode
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<MatrixNode>? Children { get; set; }
    }
}