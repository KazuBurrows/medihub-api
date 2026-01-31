public class ScheduleTemplate
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int TheatreId { get; set; }
    public int Week { get; set; }
    public byte DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
