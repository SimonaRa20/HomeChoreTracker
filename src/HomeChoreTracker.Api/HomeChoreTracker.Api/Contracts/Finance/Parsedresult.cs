namespace HomeChoreTracker.Api.Contracts.Finance
{
	public class Parsedresult
	{
		public object FileParseExitCode { get; set; }
		public string ParsedText { get; set; }
		public string ErrorMessage { get; set; }
		public string ErrorDetails { get; set; }
	}
}
