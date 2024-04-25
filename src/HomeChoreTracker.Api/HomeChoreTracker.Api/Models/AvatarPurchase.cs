namespace HomeChoreTracker.Api.Models
{
    public class AvatarPurchase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AvatarId { get; set; }

        public User User { get; set; }
        public Avatar Avatar { get; set; }
    }
}
