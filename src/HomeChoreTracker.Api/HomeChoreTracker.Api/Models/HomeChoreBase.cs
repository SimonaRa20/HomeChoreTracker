using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Models
{
    public class HomeChoreBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public HomeChoreType ChoreType { get; set; }
        public Frequency Frequency { get; set; }
        public string? Description { get; set; }
    }
}
