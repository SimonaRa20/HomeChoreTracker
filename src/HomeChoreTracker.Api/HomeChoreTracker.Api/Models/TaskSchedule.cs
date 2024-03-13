namespace HomeChoreTracker.Api.Models
{
    public class TaskSchedule
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DateTime>? Dates { get; set; }
        public bool Randomize { get; set; }
        public int? HomeMemberId { get; set; }
    }
}
