using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Finance
{
    public class FinancialCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FinancialType Type { get; set; }
        public int? HomeId { get; set; }
        public int UserId { get; set; }
    }
}
