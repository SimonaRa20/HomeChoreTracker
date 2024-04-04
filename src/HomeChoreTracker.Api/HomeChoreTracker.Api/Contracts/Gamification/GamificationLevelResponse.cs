namespace HomeChoreTracker.Api.Contracts.Gamification
{
    public class GamificationLevelResponse
    {
        public int Id { get; set; }
        public int? LevelId { get; set; }
        public int? PointsRequired { get; set; }
        public byte[]? Image { get; set; }
    }
}
