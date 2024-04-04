namespace HomeChoreTracker.Api.Models
{
    public class GamificationLevel
    {
        public int Id { get; set; }
        public int? LevelId { get; set; }
        public int? PointsRequired { get; set; }
        public byte[]? Image { get; set; }
    }
}
