using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Forum
{
	public class AdviceRequest
	{
		public string Title { get; set; }
		public HomeChoreType Type { get; set; }
		public string Description { get; set; }
		public bool IsPublic { get; set; }
	}
}
