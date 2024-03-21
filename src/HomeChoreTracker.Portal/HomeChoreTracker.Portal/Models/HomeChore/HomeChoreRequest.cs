using HomeChoreTracker.Portal.Constants;
using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.HomeChore
{
    public class HomeChoreRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public HomeChoreType ChoreType { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Points are required.")]
        public int? Points { get; set; }
        public LevelType LevelType { get; set; }
        public TimeLong Time { get; set; }
        [Required(ErrorMessage = "Interval is required.")]
        public int? Interval { get; set; }
        public RepeatUnit Unit { get; set; }
        public List<int>? DaysOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public MonthlyRepeatType? MonthlyRepeatType { get; set; }
        public int HomeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
