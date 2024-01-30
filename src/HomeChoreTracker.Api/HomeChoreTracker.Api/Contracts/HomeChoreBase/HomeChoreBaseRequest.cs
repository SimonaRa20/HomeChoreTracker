using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.HomeChoreBase
{
    public class HomeChoreBaseRequest
    {
        public string Name { get; set; }
        public HomeChoreType ChoreType { get; set; }
        public Frequency Frequency { get; set; }
        public string? Description { get; set; }
    }
}
