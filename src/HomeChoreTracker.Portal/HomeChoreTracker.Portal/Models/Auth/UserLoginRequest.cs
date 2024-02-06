using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.Auth
{
    public class UserLoginRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
