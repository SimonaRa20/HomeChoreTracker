using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.Home
{
    public class HomeRequest
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
    }
}
