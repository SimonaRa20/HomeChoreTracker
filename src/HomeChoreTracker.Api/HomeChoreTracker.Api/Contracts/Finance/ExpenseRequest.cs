using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Finance
{
	public class ExpenseRequest
	{
		public string Title { get; set; }
		public decimal Amount { get; set; }
		public string? Description { get; set; }
		public DateTime Time { get; set; }
		public ExpenseType Type { get; set; }
		public int? HomeId { get; set; }
	}
}
