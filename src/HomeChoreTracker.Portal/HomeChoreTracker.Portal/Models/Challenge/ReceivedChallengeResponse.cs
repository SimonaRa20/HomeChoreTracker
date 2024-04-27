using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Challenge
{
    public class ReceivedChallengeResponse
    {
        public int Id { get; set; }
        public OpponentType OpponentType { get; set; }
        public string? HomeTitle { get; set; }
        public string? UserName { get; set; }
        public string? OpponentHomeTitle { get; set; }
        public string? OpponentUserName { get; set; }
        public ChallengeType ChallengeType { get; set; }
        public int ChallengeCount { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }
}
