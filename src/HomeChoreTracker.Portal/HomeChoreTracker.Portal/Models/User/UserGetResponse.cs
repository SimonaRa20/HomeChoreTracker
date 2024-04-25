namespace HomeChoreTracker.Portal.Models.User
{
    public class UserGetResponse
    {
        public int? HomeMemberId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
		public byte[]? Image { get; set; }

		public TimeSpan StartDayTime { get; set; }
        public TimeSpan EndDayTime { get; set; }
    }
}
