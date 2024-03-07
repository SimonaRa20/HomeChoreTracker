using DocumentFormat.OpenXml.Drawing.Charts;
using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Models
{
	public class HomeChoreTask
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }
		public int Points { get; set; }
		public LevelType Level { get; set; }
		public HomeChoreType ChoreType { get; set; }
		public TimeLong Time { get; set; }
		public bool IsActive { get; set; }
		public bool IsDone { get; set; }
		public bool IsAprroved { get; set; }

		public int HomeId { get; set; }
	}
}
