namespace HomeChoreTracker.Portal.Models.HomeChore
{
    public class HomeChoreEventResponse
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string ChoreType { get; set; }
        public string? Description { get; set; }
        public int Points { get; set; }
        public string LevelType { get; set; }
		public int HoursTime { get; set; }
		public int MinutesTime { get; set; }
		public bool IsDone { get; set; }
        public string Product { get; set; }
    }
}
