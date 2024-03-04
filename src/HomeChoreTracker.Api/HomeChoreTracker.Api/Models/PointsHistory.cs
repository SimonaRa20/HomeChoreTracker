namespace HomeChoreTracker.Api.Models
{
	public class PointsHistory
	{
		public int Id { get; set; }
		public int EarnedPoints { get; set; }
		public int HomeMemberId {  get; set; } 
		public int TaskId { get; set; }
		public DateTime EarnedDate { get; set; }
	}
}
