namespace HomeChoreTracker.Api.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<Event> Events { get; set; }
        public List<TaskAssignment> TaskAssignments { get; set; } 
    }
}
