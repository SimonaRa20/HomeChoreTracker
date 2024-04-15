using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Models
{
	public class Advice
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public DateTime Time { get; set; }
		public HomeChoreType Type { get; set; }
		public string Description { get; set; }
		public bool IsPublic { get; set; }

		public int UserId { get; set; }
		public User User { get; set; }
	}
}
