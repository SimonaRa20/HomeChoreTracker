using HomeChoreTracker.Api.Constants;
using DayOfWeek = HomeChoreTracker.Api.Constants.DayOfWeek;
namespace HomeChoreTracker.Api.Contracts.HomeChore
{
    public class HomeChoreRequest
    {
        public string Name { get; set; }
        public HomeChoreType ChoreType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public int Points { get; set; }
        public LevelType LevelType { get; set; }
		public int HoursTime { get; set; }
		public int MinutesTime { get; set; }
		public int Interval { get; set; }
        public RepeatUnit Unit { get; set; }
        public List<int>? DaysOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public MonthlyRepeatType? MonthlyRepeatType { get; set; }
        public int HomeId { get; set; }
        public bool WasEarnedPoints { get; set; }
        public bool IsPublic { get; set; }
    }
}
