namespace HomeChoreTracker.Api.Contracts.Home
{
    public class InvitationAnswerRequest
    {
        public string Token { get; set; }
        public bool IsAccept {  get; set; }
    }
}
