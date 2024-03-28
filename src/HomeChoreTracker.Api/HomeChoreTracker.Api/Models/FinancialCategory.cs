using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Models
{
    public class FinancialCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FinancialType Type { get; set; }
        public int? HomeId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
