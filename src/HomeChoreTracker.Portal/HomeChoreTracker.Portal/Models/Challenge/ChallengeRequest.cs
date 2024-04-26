using HomeChoreTracker.Portal.Constants;
using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.Challenge
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
        [Range(0, int.MaxValue, ErrorMessage = "At least one of Days, Hours, Minutes, or Seconds must be more than 0.")]
        public int Days { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "At least one of Days, Hours, Minutes, or Seconds must be more than 0.")]
        public int Hours { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "At least one of Days, Hours, Minutes, or Seconds must be more than 0.")]
        public int Minutes { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "At least one of Days, Hours, Minutes, or Seconds must be more than 0.")]
        public int Seconds { get; set; }
    }
}
