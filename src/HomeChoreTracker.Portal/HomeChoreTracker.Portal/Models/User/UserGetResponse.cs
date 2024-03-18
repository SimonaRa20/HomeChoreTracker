namespace HomeChoreTracker.Portal.Models.User
{
    public class UserGetResponse
    {
        public int? HomeMemberId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
