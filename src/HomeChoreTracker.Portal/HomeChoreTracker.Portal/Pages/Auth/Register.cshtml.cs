using HomeChoreTracker.Portal.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeChoreTracker.Portal.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        public UserRegisterRequestForm UserRegisterRequest { get; set; }

        public RegisterModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var apiUrl = _config["ApiUrl"] + "/Auth/Register";

                UserRegisterRequest userRegisterRequest = new UserRegisterRequest
                {
                    UserName = UserRegisterRequest.UserName,
                    Email = UserRegisterRequest.Email,
                    Password = UserRegisterRequest.Password,
                };

                var response = await httpClient.PostAsJsonAsync(apiUrl, userRegisterRequest);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Auth/Login");
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
