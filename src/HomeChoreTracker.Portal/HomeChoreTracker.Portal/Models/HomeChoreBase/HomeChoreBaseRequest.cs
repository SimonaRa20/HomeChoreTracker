using HomeChoreTracker.Portal.Models.HomeChoreBase.Constants;
using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.HomeChoreBase
{
    public class HomeChoreBaseRequest
    {
        public string Name { get; set; }
        public int ChoreType { get; set; }
        public int Frequency { get; set; }
        public string? Description { get; set; }
    }
}
