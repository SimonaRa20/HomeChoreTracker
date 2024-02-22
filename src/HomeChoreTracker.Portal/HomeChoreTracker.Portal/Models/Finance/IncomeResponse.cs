namespace HomeChoreTracker.Portal.Models.Finance
{
    public class IncomeResponse
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public string? Home { get; set; }
    }
}
