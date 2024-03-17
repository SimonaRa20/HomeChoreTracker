namespace HomeChoreTracker.Api.Contracts.HomeChore
{
    public class SetHomeChoreDatesRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
