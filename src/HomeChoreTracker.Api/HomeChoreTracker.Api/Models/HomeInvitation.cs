namespace HomeChoreTracker.Api.Models
{
    public class HomeInvitation
    {
        public int Id { get; set; }
        public string InviteeEmail { get; set; }
        public string InvitationToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsAccepted { get; set; }
        
        public int HomeId { get; set; }
        public Home? Home { get; set; }
        public int InviterUserId { get; set; }
        public User? InviterUser { get; set; }
    }
}
