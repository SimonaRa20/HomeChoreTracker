using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.HomeChoreBase
{
    public class HomeChoreBaseRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public int ChoreType { get; set; }
		public string? Description { get; set; }
        [Required(ErrorMessage = "Points are required.")]
        public int? Points { get; set; }
		public int LevelType { get; set; }
		public int Time { get; set; }
        [Required(ErrorMessage = "Interval is required.")]
        public int? Interval { get; set; }
		public int Unit { get; set; }
		public List<int> DaysOfWeek { get; set; }
		public int? DayOfMonth { get; set; }
		public int? MonthlyRepeatType { get; set; }
	}
}
