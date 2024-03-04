namespace HomeChoreTracker.Api.Models
{
    public class Home
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public List<Purchase> Purchases { get; set; }
        public List<HomeChoreTask> Tasks { get; set; }
    }
}
