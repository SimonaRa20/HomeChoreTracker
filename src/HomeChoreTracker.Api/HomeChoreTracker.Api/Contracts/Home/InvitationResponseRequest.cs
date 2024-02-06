namespace HomeChoreTracker.Api.Contracts.Home
{
    public class InvitationResponseRequest
    {
        public string InvitationToken { get; set; }
        public bool AcceptInvitation { get; set; }
    }
}
