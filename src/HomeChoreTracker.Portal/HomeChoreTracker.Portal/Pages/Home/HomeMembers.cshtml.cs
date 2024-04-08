using HomeChoreTracker.Portal.Models.Home;
using HomeChoreTracker.Portal.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Home
{
    public class HomeMembersModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<UserGetResponse> HomeMembers { get; set; }
        public bool Unauthorized { get; set; }

        public HomeMembersModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/Home/HomeMembers?homeId={id}";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    HomeMembers = await response.Content.ReadFromJsonAsync<List<UserGetResponse>>();
                    return Page();
                }
                else if(response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Unauthorized = true;
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {response.ReasonPhrase}");
                    return Page();
                }
            }
        }
    }
}
