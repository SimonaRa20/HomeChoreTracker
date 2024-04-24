using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Avatar
{
	public class AvatarRequest
	{
		public string? AvatarType { get; set; }
		public IFormFile? Image { get; set; }
	}
}
