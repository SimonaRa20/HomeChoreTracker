namespace HomeChoreTracker.Api.Contracts.Home
{
    public class InviteUserRequest
    {
        public int HomeId { get; set; }
        public string InviteeEmail { get; set; }
    }
}
