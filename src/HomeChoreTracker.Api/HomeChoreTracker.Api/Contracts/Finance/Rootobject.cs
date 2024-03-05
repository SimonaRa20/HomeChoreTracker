namespace HomeChoreTracker.Api.Contracts.Finance
{
	public class Rootobject
	{
		public Parsedresult[] ParsedResults { get; set; }
		public int OCRExitCode { get; set; }
		public bool IsErroredOnProcessing { get; set; }
		public string ErrorMessage { get; set; }
		public string ErrorDetails { get; set; }
	}
}
