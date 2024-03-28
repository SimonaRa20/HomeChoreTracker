using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Finance
{
    public class IncomeResponse
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Time { get; set; }
        public FinancialType Type { get; set; }
        public string? FinancialCategory { get; set; }
        public int FinancialCategoryId { get; set; }
        public int? HomeId { get; set; }
    }
}
