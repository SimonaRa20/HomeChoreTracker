using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Finance
{
	public class ExpenseImageRequest
	{
		public ExpenseType Type { get; set; }
		public int? HomeId { get; set; }
		public IFormFile ExpenseImage { get; set; }
	}
}
