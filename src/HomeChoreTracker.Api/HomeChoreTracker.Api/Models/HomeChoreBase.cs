using HomeChoreTracker.Api.Constants;
using DayOfWeek = HomeChoreTracker.Api.Constants.DayOfWeek;

namespace HomeChoreTracker.Api.Models
{
    public class HomeChoreBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public HomeChoreType ChoreType { get; set; }
        public string? Description { get; set; }
		public LevelType LevelType { get; set; }
		public TimeLong Time {  get; set; }
		public int Interval { get; set; }
		public RepeatUnit Unit { get; set; }
		public List<DayOfWeek>? DaysOfWeek { get; set; }
		public int? DayOfMonth { get; set; }
		public MonthlyRepeatType? MonthlyRepeatType { get; set; }
	}
}
