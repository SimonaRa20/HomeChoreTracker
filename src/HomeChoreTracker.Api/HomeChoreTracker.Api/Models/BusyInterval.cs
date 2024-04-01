namespace HomeChoreTracker.Api.Models
{
    public class BusyInterval
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
