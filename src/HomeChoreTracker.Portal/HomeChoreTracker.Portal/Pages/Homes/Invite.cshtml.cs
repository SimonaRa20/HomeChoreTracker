using HomeChoreTracker.Portal.Models.Auth;
using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeChoreTracker.Portal.Pages.Homes
{
    public class InviteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        public InvitationResponseRequest InvitationResponseRequest { get; set; }

        public InviteModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public IActionResult OnGet()
        {
            InvitationResponseRequest = new InvitationResponseRequest();
            InvitationResponseRequest.Token = Request.Query["token"];
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
                var apiUrl = _config["ApiUrl"] + "/Home/InvitationAnswer";
                var response = await httpClient.PostAsJsonAsync(apiUrl, InvitationResponseRequest);

                if (response.IsSuccessStatusCode)
                {
                    var isAuthenticated = User.Identity.IsAuthenticated;

                    if (isAuthenticated)
                    {
                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        return RedirectToPage("/Auth/Login");
                    }
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
