namespace HomeChoreTracker.Portal.Models.Gamification
{
    public class GamificationLevelRequest
    {
        public int? LevelId { get; set; }
        public int? PointsRequired { get; set; }
        public IFormFile? Image { get; set; }
    }
}
