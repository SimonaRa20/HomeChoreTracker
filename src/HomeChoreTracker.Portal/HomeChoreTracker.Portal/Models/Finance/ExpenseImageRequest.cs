using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Finance
{
	public class ExpenseImageRequest
	{
        public int FinancialCategoryId { get; set; }
        public int? HomeId { get; set; }
        public string? NewFinancialCategory { get; set; }
        public IFormFile ExpenseImage { get; set; }
    }
}
