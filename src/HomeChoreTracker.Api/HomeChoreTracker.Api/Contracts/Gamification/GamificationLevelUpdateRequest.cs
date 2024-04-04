namespace HomeChoreTracker.Api.Contracts.Gamification
{
    public class GamificationLevelUpdateRequest
    {
        public int? PointsRequired { get; set; }
        public IFormFile? Image { get; set; }
    }
}
