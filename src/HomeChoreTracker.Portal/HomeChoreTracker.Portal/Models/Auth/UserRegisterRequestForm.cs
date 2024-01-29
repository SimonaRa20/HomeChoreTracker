using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Models.Auth
{
    public class UserRegisterRequestForm
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Required(ErrorMessage = "Repeat Password is required.")]
        public string RepeatPassword { get; set; }
    }
}
