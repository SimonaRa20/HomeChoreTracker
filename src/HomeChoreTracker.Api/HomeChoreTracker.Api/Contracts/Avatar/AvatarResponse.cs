using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Avatar
{
	public class AvatarResponse
	{
		public int Id { get; set; }
		public AvatarType AvatarType { get; set; }
		public byte[]? Image { get; set; }
	}
}
