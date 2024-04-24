using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Avatar
{
    public class AvatarResponse
    {
        public int Id { get; set; }
        public AvatarType AvatarType { get; set; }
        public byte[]? Image { get; set; }
    }
}
