using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.HomeChoreBase
{
    public class HomeChoreBaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public HomeChoreType ChoreType { get; set; }
        public Frequency Frequency { get; set; }
        public string? Description { get; set; }
    }
}
