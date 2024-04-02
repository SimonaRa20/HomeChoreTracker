namespace HomeChoreTracker.Api.Contracts.User
{
    public class BusyIntervalRequest
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
