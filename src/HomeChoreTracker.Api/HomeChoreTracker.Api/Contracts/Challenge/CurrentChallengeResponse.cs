using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Challenge
{
	public class CurrentChallengeResponse
	{
		public int Id { get; set; }
		public OpponentType OpponentType { get; set; }
		public string? HomeTitle { get; set; }
		public string? UserName { get; set; }
		public string? OpponentHomeTitle { get; set; }
		public string? OpponentUserName { get; set; }
		public ChallengeType ChallengeType { get; set; }
		public int ChallengeCount { get; set; }
		public int Count { get; set; }
		public int OpponentCount { get; set; }
		public DateTime EndTime { get; set; }
	}
}
