using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Contracts.Home
{
    public class LevelRequest
    {
        public int EarnedPoints { get; set; }
        public int MaxPoints { get; set; }
        public int GamificationLevelId { get; set; }
        public GamificationLevel GamificationLevel { get; set; }
    }
}
