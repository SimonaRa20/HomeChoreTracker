using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Models
{
    public class Challenge
    {
        public int Id { get; set; }
        public OpponentType OpponentType { get; set; }
        public int? HomeId { get; set; }
        public int? UserId { get; set; }
        public int? OpponentHomeId { get; set; }
        public int? OpponentUserId { get; set; }
        public ChallengeType ChallengeType { get; set; }
        public int ChallengeCount { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ChallengeInvitationType Action { get; set; }
        public ChallengeResultType ResultType { get; set; }
    }
}
