using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Avatar
{
	public class AvatarRequest
	{
		public AvatarType AvatarType { get; set; }
		public IFormFile? Image { get; set; }
	}
}
