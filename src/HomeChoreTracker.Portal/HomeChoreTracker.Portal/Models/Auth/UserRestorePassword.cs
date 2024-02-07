using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.Auth
{
    public class UserRestorePassword
    {
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
