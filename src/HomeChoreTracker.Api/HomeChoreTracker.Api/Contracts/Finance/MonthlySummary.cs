namespace HomeChoreTracker.Api.Contracts.Finance
{
	public class MonthlySummary
	{
		public string MonthYear { get; set; }
		public decimal TotalIncome { get; set; }
		public decimal TotalExpense { get; set; }
	}
}
