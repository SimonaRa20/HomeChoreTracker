using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Forum
{
    public class AdviceDetailedResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public HomeChoreType Type { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }

        public int UserId { get; set; }
    }
}
