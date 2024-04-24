using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Avatar
{
	public class AvatarUpdateRequest
	{
		public AvatarType AvatarType { get; set; }
		public IFormFile? Image { get; set; }
	}
}
