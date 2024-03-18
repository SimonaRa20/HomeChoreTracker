namespace HomeChoreTracker.Portal.Models.HomeChore
{
    public class TaskAssignmentResponse
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public int TaskId { get; set; }
        public int? HomeMemberId { get; set; }
        public int HomeId { get; set; }
        public bool IsDone { get; set; }
        public bool IsApproved { get; set; }
    }
}
