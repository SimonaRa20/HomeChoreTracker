namespace HomeChoreTracker.Api.Models
{
	public class TaskApproval
	{
		public int Id { get; set; }
		public int TaskAssignmentId { get; set; }
		public int HomeMemberId { get; set; }  // have to be home admin
	}
}
