namespace HomeChoreTracker.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

		public List<Income> Incomes { get; set; }
		public List<Expense> Expenses { get; set; }
        public List<Event> CalendarEvents { get; set; }
	}
}
