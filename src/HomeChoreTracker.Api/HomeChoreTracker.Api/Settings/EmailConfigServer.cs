namespace HomeChoreTracker.Api.Settings
{
	public class EmailConfigServer
	{
		public const string SectionName = nameof(EmailConfigServer);

		public string Server { get; set; }
		public int Port { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
	}
}
