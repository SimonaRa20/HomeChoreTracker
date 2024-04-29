namespace HomeChoreTracker.Api.Settings
{
	public class JwtSettings
	{
		public const string SectionName = nameof(JwtSettings);

		public string Key { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; }
	}
}
