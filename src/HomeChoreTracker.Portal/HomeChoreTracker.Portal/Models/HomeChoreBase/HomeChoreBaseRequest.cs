using HomeChoreTracker.Portal.Models.HomeChoreBase.Constants;

namespace HomeChoreTracker.Portal.Models.HomeChoreBase
{
    public class HomeChoreBaseRequest
    {
        public string Name { get; set; }
        public HomeChoreType ChoreType { get; set; }
        public Frequency Frequency { get; set; }
        public string? Description { get; set; }
    }
}
