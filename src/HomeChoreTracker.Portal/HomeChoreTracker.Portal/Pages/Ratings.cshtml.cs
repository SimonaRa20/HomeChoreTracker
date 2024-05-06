using HomeChoreTracker.Portal.Models.Challenge;
using HomeChoreTracker.Portal.Models.Gamification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages
{
    public class RatingsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

		public ChallengeMVPRequest GetUserChallengeMVP { get; set; }
		public ChallengeMVPRequest GetHomeChallengeMVP { get; set; }
		public List<RatingsResponse> GetRatingsByBadges { get; set; }
        public List<RatingsResponse> GetRatingsByPoints { get; set; }
        public int UserId { get; set; }

        public RatingsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = _config["ApiUrl"] + "/Gamification/RatingsByBadges";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    GetRatingsByBadges = await response.Content.ReadFromJsonAsync<List<RatingsResponse>>();
                    var apiUrlGetRatingsByPoints = _config["ApiUrl"] + "/Gamification/RatingsByPoints";
                    var responseGetRatingsByPoints = await httpClient.GetAsync(apiUrlGetRatingsByPoints);

                    if (responseGetRatingsByPoints.IsSuccessStatusCode)
                    {
                        GetRatingsByPoints = await responseGetRatingsByPoints.Content.ReadFromJsonAsync<List<RatingsResponse>>();

						var apiUrlGetUserChallenges = _config["ApiUrl"] + "/Challenge/UserChallengesMVP";
						var responseGetUserChallenges = await httpClient.GetAsync(apiUrlGetUserChallenges);
						if (responseGetUserChallenges.IsSuccessStatusCode)
                        {
							GetUserChallengeMVP = await responseGetUserChallenges.Content.ReadFromJsonAsync<ChallengeMVPRequest>();
							var apiUrlGetHomeChallenges = _config["ApiUrl"] + "/Challenge/HomeChallengesMVP";
							var responseGetHomeChallenges = await httpClient.GetAsync(apiUrlGetHomeChallenges);
							if (responseGetHomeChallenges.IsSuccessStatusCode)
							{
								GetHomeChallengeMVP = await responseGetHomeChallenges.Content.ReadFromJsonAsync<ChallengeMVPRequest>();
							}
						}
					}
                    return Page();
                }
                else
                {
                    return Page();
                }
            }
        }
    }
}
