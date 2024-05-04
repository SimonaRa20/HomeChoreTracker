using HomeChoreTracker.Portal.Constants;
using HomeChoreTracker.Portal.Models.Avatar;
using HomeChoreTracker.Portal.Models.Challenge;
using HomeChoreTracker.Portal.Models.HomeChore;
using HomeChoreTracker.Portal.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Challenges
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        public ChallengeRequest CreateChallenge { get; set; }

        public List<UserResponse> UserResponses { get; set; }
		public List<HomeResponse> HomeUserResponses { get; set; }
		public List<HomeResponse> HomeOpponentsResponses { get; set; }
        public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
                var apiUrl = $"{_config["ApiUrl"]}/Challenge/OpponentsUsers";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    UserResponses = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
                    var apiHomeUrl = $"{_config["ApiUrl"]}/Challenge/UserHomes";
                    var responseHome = await httpClient.GetAsync(apiHomeUrl);

                    if (responseHome.IsSuccessStatusCode)
                    {
                        HomeUserResponses = await responseHome.Content.ReadFromJsonAsync<List<HomeResponse>>();
                        var apiHomeOpponentUrl = $"{_config["ApiUrl"]}/Challenge/OpponentsHomes";
                        var responseOpponentHome = await httpClient.GetAsync(apiHomeOpponentUrl);

                        if (responseOpponentHome.IsSuccessStatusCode)
                        {
                            HomeOpponentsResponses = await responseOpponentHome.Content.ReadFromJsonAsync<List<HomeResponse>>();
                        }
                    }

                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {response.ReasonPhrase}");
                    return Page();
                }
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var token = User.FindFirstValue("Token");
                    using (var httpClient = _httpClientFactory.CreateClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var apiUrl = _config["ApiUrl"] + "/Challenge";
						var type = Enum.Parse<OpponentType>(Request.Form["CreateAvatar.OpponentType"]);
                        CreateChallenge.OpponentType = type;
						var response = await httpClient.PostAsJsonAsync(apiUrl, CreateChallenge);

                        if (response.IsSuccessStatusCode)
                        {
							TempData["ToastType"] = "success";
							TempData["ToastMessage"] = "Challenge created successfully.";
                            await OnGetAsync();
							return Page();
						}
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
                            return Page();
                        }
                    }
                }
                else
                {
                    return RedirectToPage("/");
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}
