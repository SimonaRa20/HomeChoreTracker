using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Models
{
	public class Income
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public decimal Amount { get; set; }
		public string? Description { get; set; }
		public DateTime Time { get; set; }
		public IncomeType Type { get; set; }
		public int? SubscriptionDuration { get; set; }
		public int? HomeId { get; set; }

		public int UserId { get; set; }
	}
}
