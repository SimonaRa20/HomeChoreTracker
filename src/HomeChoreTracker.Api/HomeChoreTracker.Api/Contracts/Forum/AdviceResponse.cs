using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Forum
{
	public class AdviceResponse
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public DateTime Time { get; set; }
		public HomeChoreType Type { get; set; }
		public string Description { get; set; }
		public bool IsPublic { get; set; }
		public string UserName { get; set; }
		public int UserId { get; set; }
	}
}
