namespace HomeChoreTracker.Api.Models
{
	public class TaskAssignment
	{
		public int Id { get; set; }
		public DateTime SetDate { get; set; }
		public int HomeChoreTaskId { get; set; }
		public int HomeMemberId { get; set; }
	}
}
