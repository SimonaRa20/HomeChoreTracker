namespace HomeChoreTracker.Api.Models
{
	public class TaskAssignment
	{
		public int Id { get; set; }
		public DateTime SetDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int HomeChoreTaskId { get; set; }
		public int? HomeMemberId { get; set; }
		public bool IsDone { get; set; }
		public bool IsApproved { get; set; }
	}
}
