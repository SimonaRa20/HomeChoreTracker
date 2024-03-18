namespace HomeChoreTracker.Portal.Models.Calendar
{
    public class Event
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Summary { get; set; }
    }
}
