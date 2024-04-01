namespace HomeChoreTracker.Portal.Models.Profile
{
    public class UserProfileResponse
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }

        public TimeSpan StartDayTime { get; set; }
        public TimeSpan EndDayTime { get; set; }
    }
}
