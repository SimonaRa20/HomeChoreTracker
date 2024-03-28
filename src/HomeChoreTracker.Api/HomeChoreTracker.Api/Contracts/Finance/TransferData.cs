using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Contracts.Finance
{
    public class TransferData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public int Type { get; set; }
		public FinancialCategory Category { get; set; }
		public int? HomeId { get; set; }
        public int UserId { get; set; }
    }
}
