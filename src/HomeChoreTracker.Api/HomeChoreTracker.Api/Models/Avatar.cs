using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Models
{
	public class Avatar
	{
		public int Id { get; set; }
		public AvatarType AvatarType { get; set; }
		public byte[]? Image { get; set; }
	}
}
