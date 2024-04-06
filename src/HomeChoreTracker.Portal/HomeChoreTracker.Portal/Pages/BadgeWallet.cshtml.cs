using HomeChoreTracker.Portal.Models.Gamification;
using HomeChoreTracker.Portal.Models.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages
{
    public class BadgeWalletModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public BadgeWalletResponse BadgeWallet { get; set; }

        public BadgeWalletModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = _config["ApiUrl"] + "/Gamification/BadgeWallet";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    BadgeWallet = await response.Content.ReadFromJsonAsync<BadgeWalletResponse>();
                    return Page();
                }
                else
                {
                    // Handle error scenario
                    return Page();
                }
            }
        }
    }
}
