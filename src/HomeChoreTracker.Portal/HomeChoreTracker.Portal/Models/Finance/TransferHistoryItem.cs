namespace HomeChoreTracker.Portal.Models.Finance
{
    public class TransferHistoryItem
    {
        public string Type { get; set; } // Either "Income" or "Expense"
        public TransferData Data { get; set; } // Contains the actual income or expense data
    }
}
