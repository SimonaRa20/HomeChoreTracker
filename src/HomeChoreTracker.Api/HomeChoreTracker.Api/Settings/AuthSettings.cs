namespace HomeChoreTracker.Api.Settings
{
	public class AuthSettings
	{
		public const string SectionName = nameof(AuthSettings);

		public string Salt { get; set; }
		public string AppUrl { get; set; }
	}
}
