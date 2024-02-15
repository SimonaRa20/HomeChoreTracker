using HomeChoreTracker.Portal.Constants;
using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.HomeChoreBase
{
    public class HomeChoreBaseEditRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public HomeChoreType ChoreType { get; set; }
        public Frequency Frequency { get; set; }
        public string? Description { get; set; }
    }
}
