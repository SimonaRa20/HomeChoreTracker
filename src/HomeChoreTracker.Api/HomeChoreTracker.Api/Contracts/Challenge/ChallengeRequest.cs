using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Challenge
{
    public class ChallengeRequest
    {
        public OpponentType OpponentType { get; set; }
        public int? HomeId { get; set; }
        public int? UserId { get; set; }
        public int? OpponentHomeId { get; set; }
        public int? OpponentUserId { get; set; }
        public ChallengeType ChallengeType { get; set; }
        public int ChallengeCount { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }
}
