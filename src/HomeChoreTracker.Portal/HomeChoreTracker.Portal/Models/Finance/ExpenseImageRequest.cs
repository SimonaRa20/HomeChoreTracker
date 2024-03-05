using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Finance
{
	public class ExpenseImageRequest
	{
		public IFormFile ExpenseImage { get; set; }
		public ExpenseType Type { get; set; }
		public int? HomeId { get; set; }
	}
}
