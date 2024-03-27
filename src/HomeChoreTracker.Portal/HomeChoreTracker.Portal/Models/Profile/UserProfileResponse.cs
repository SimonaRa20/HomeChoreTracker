namespace HomeChoreTracker.Portal.Models.Profile
{
    public class UserProfileResponse
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int StartDayHour { get; set; }
        public int StartDayMinutes { get; set; }
        public int StartLunchHour { get; set; }
        public int StartLunchMinutes { get; set; }
        public int EndLunchHour { get; set; }
        public int EndLunchMinutes { get; set; }
        public int EndDayHour { get; set; }
        public int EndDayMinutes { get; set; }
        public bool Morning { get; set; }
        public bool MiddleDay { get; set; }
        public bool Evening { get; set; }
    }
}
