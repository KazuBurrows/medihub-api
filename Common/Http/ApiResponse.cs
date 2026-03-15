public class ApiResponse
{
    public string Title { get; set; } = "";
    public int Status { get; set; }
    public string Detail { get; set; } = "";
    public Dictionary<string, object>? Extensions { get; set; }
}