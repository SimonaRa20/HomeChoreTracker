namespace HomeChoreTracker.Api.Models
{
    public class UserHomes
    {
        public int UserId { get; set; }
        public int HomeId { get; set; }
        public string? HomeRole { get; set; }

        public User User { get; set; }
        public Home Home { get; set; }
    }
}
