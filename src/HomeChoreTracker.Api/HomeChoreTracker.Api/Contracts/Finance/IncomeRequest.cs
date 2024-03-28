using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Contracts.Finance
{
	public class IncomeRequest
	{
		public string Title { get; set; }
		public decimal Amount { get; set; }
		public string? Description { get; set; }
		public DateTime Time { get; set; }
		public FinancialType Type { get; set; }
        public int FinancialCategoryId { get; set; }

        public int? HomeId { get; set; }
		public string? NewFinancialCategory { get; set; }
	}
}
