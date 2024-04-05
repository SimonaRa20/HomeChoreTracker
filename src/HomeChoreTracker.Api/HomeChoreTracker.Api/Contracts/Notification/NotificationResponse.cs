namespace HomeChoreTracker.Api.Contracts.Notification
{
	public class NotificationResponse
	{
		public string Title { get; set; }
		public bool IsRead { get; set; }
		public DateTime Time { get; set; }
	}
}
