using HomeChoreTracker.Portal.Models.Gamification;
using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Home
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public LevelRequest LevelResponse { get; set; }
        public Dictionary<string, int> ThisWeekPointsHistory { get; set; }
        public Dictionary<string, int> PreviousWeekPointsHistory { get; set; }
        public bool Unauthorized { get; set; }
        public int Level { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

                var apiUrl = $"{_config["ApiUrl"]}/Home/Level/{id}";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    LevelResponse = await response.Content.ReadFromJsonAsync<LevelRequest>();

                    var apiUrlThisWeek = $"{_config["ApiUrl"]}/Gamification/ThisWeek/{id}";

                    var responseThisWeek = await httpClient.GetAsync(apiUrlThisWeek);

                    if (responseThisWeek.IsSuccessStatusCode) {

                        ThisWeekPointsHistory = await responseThisWeek.Content.ReadFromJsonAsync<Dictionary<string, int>>();
                    }


					var apiUrlLevel = $"{_config["ApiUrl"]}/Home/Level";

					var responseLevel = await httpClient.GetAsync(apiUrlLevel);

					if (responseLevel.IsSuccessStatusCode)
					{

						Level = await responseLevel.Content.ReadFromJsonAsync<int>();
					}

					var apiUrlPreviousWeek = $"{_config["ApiUrl"]}/Gamification/PreviousWeek/{id}";

                    var responsePreviousWeek = await httpClient.GetAsync(apiUrlPreviousWeek);

                    if (responsePreviousWeek.IsSuccessStatusCode)
                    {
                        PreviousWeekPointsHistory = await responsePreviousWeek.Content.ReadFromJsonAsync<Dictionary<string, int>>();
                    }

                    return Page();
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Unauthorized = true;
                    return Page();
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }
        public async Task<IActionResult> OnPostAsync(int homeId)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/Home/Level/{homeId}";

                var response = await httpClient.PutAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Home/Index", new { id = homeId });
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }
    }
}
