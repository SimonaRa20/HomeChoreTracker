namespace HomeChoreTracker.Api.Models
{
    public class TaskVote
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public int UserId { get; set; }

        public int TaskAssignmentId { get; set; }
        public TaskAssignment? TaskAssignment { get; set; }
    }
}
