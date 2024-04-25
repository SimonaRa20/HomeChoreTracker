using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Avatar
{
    public class AvatarSelectionResponse
    {
        public int Id { get; set; }
        public AvatarType AvatarType { get; set; }
        public byte[]? Image { get; set; }
        public bool IsPurchased { get; set; }
    }
}
