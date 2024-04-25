namespace HomeChoreTracker.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

        public TimeSpan StartDayTime { get; set; }
        public TimeSpan EndDayTime { get; set; }

        public int? AvatarId { get; set; }
        public Avatar Avatar { get; set; }

        public List<Advice> Advices { get; set; }
        public List<PointsHistory> PointsHistories { get; set; }
        public List<UserHomes>? UserHomes { get; set; }
        public List<FinancialRecord>? FinancialRecords { get; set; }
        public List<Event>? CalendarEvents { get; set; }
        public List<BusyInterval> BusyIntervals { get; set; }
        public List<Notification> Notifications { get; set; }
        public BadgeWallet BadgeWallet { get; set; }
        public List<AvatarPurchase> AvatarPurchase { get; set; }
    }
}
