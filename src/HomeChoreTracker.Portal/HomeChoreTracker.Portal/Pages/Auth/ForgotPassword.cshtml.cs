using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace HomeChoreTracker.Portal.Pages.Auth
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ForgotPasswordModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var apiUrl = _config["ApiUrl"] + "/Auth/RestorePassword";
                var response = await httpClient.PostAsJsonAsync(apiUrl, Email);

                if (response.IsSuccessStatusCode)
                {
                    TempData["ToastType"] = "success";
                    TempData["ToastMessage"] = "Password update email has been sent.";

                    return Page();
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
