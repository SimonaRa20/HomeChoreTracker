using HomeChoreTracker.Portal.Constants;
using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.HomeChoreBase
{
    public class HomeChoreBaseEditRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public int ChoreType { get; set; }
        public string? Description { get; set; }
        public int LevelType { get; set; }
        public int Time { get; set; }
        public int Interval { get; set; }
        public int Unit { get; set; }
        public List<int>? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public int? MonthlyRepeatType { get; set; }
    }
}
