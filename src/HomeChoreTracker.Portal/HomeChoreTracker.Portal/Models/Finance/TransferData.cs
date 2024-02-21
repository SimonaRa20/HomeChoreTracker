namespace HomeChoreTracker.Portal.Models.Finance
{
    public class TransferData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public int Type { get; set; } // You might want to change this to enum or string based on your IncomeType and ExpenseType
        public int? SubscriptionDuration { get; set; }
        public int? HomeId { get; set; }
        public int UserId { get; set; }
    }
}
