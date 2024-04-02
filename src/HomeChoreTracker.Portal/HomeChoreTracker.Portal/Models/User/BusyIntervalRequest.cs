using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.User
{
    public class BusyIntervalRequest
    {
        public DayOfWeek Day { get; set; }
        [Required(ErrorMessage = "StartTime is required.")]
        public TimeSpan StartTime { get; set; }
        [Required(ErrorMessage = "EndTime is required.")]
        public TimeSpan EndTime { get; set; }
    }
}
