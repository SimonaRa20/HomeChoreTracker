using HomeChoreTracker.Portal.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Pages.Auth
{
    public class RestorePasswordModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }

        [BindProperty]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string Token { get; set; }

        private UserRestorePassword userRestorePassword { get; set; } = new UserRestorePassword();

        public RestorePasswordModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public IActionResult OnGet()
        {
            Token = Request.Query["token"];
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var apiUrl = _config["ApiUrl"] + "/Auth/RestoreForgotPassword";

                userRestorePassword.Token = Token;
                userRestorePassword.Password = NewPassword;

                var response = await httpClient.PutAsJsonAsync(apiUrl, userRestorePassword);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Login");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return Page();
                }
            }
        }
    }
}
