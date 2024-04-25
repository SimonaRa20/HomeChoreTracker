namespace HomeChoreTracker.Api.Models
{
	public class PointsHistory
	{
		public int Id { get; set; }
		public int EarnedPoints { get; set; }
		
		public int? HomeId { get; set; }
		public int? TaskId { get; set; }
		public string Text { get; set; }
		public DateTime EarnedDate { get; set; }

		public int? HomeMemberId { get; set; }
		public User User { get; set; }
	}
}
