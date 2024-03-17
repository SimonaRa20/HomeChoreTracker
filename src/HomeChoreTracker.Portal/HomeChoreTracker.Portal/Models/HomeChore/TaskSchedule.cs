namespace HomeChoreTracker.Portal.Models.HomeChore
{
    public class TaskSchedule
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
