namespace HomeChoreTracker.Portal.Models.Gamification
{
    public class GamificationLevelUpdateRequest
    {
        public int? PointsRequired { get; set; }
        public IFormFile? Image { get; set; }
    }
}
