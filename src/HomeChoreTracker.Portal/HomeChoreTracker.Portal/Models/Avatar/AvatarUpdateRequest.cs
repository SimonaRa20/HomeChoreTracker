using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Avatar
{
	public class AvatarUpdateRequest
	{
		public AvatarType AvatarType { get; set; }
		public IFormFile? Image { get; set; }
	}
}
