namespace HomeChoreTracker.Api.Models
{
	public class TaskAssignment
	{
		public int Id { get; set; }
		public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TaskId { get; set; }
		public int? HomeMemberId { get; set; }
		public int HomeId { get; set; }
		public int? Points { get; set; }
		public bool IsDone { get; set; }
        public List<TaskVote> TaskVotes { get; set; }
        public bool IsApproved { get; set; }
	}
}
