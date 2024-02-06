using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.HomeChoreBase
{
    public class HomeChoreBaseRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public Constants.HomeChoreType ChoreType { get; set; }
        public Constants.Frequency Frequency { get; set; }
        public string? Description { get; set; }
    }
}
