using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Finance
{
    public class IncomeResponse
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Time { get; set; }
        public IncomeType Type { get; set; }
        public int? HomeId { get; set; }
    }
}
