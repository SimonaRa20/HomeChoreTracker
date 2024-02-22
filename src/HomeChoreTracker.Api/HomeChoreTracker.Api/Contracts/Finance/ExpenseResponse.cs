using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Finance
{
    public class ExpenseResponse
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Time { get; set; }
        public ExpenseType Type { get; set; }
        public int? SubscriptionDuration { get; set; }
        public string? Home { get; set; }
    }
}
