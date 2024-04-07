namespace HomeChoreTracker.Api.Contracts.Gamification
{
    public class RatingsResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int EarnedPoints { get; set; }
        public int EarnedBadgesCount { get; set; }
    }
}
