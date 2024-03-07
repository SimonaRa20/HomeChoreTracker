using HomeChoreTracker.Portal.Constants;
using DayOfWeek = HomeChoreTracker.Portal.Constants.DayOfWeek;

namespace HomeChoreTracker.Portal.Models.HomeChoreBase
{
    public class HomeChoreBaseResponse
    {
        public int Id { get; set; }
		public string Name { get; set; }
		public HomeChoreType ChoreType { get; set; }
		public string? Description { get; set; }
		public LevelType LevelType { get; set; }
		public TimeLong Time { get; set; }
		public int Interval { get; set; }
		public RepeatUnit Unit { get; set; }
		public List<DayOfWeek>? DaysOfWeek { get; set; }
		public int? DayOfMonth { get; set; }
		public MonthlyRepeatType? MonthlyRepeatType { get; set; }
	}
}
