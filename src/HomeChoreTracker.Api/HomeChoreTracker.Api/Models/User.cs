namespace HomeChoreTracker.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

        public TimeSpan StartDayTime { get; set; }
        public TimeSpan EndDayTime { get; set; }

        public List<UserHomes>? UserHomes { get; set; }
        public List<FinancialRecord>? FinancialRecords { get; set; }
        public List<Event>? CalendarEvents { get; set; }
        public List<BusyInterval> BusyIntervals { get; set; }
    }
}
