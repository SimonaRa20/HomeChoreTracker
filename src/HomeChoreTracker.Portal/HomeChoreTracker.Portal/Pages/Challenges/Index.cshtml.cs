using HomeChoreTracker.Portal.Models.Avatar;
using HomeChoreTracker.Portal.Models.Challenge;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Challenges
{
    public class IndexModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public List<ReceivedChallengeResponse> ChallengesResponse { get; set; }
		public List<CurrentChallengeResponse> CurrentChallengeResponses { get; set; }

		public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

				var apiUrl = $"{_config["ApiUrl"]}/Challenge/ReceivedChallenges";

				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					ChallengesResponse = await response.Content.ReadFromJsonAsync<List<ReceivedChallengeResponse>>();
					var apiUrlCurrent = $"{_config["ApiUrl"]}/Challenge/CurrentChallenges";
					var currentresponse = await httpClient.GetAsync(apiUrlCurrent);
					if(currentresponse.IsSuccessStatusCode)
					{
						CurrentChallengeResponses = await currentresponse.Content.ReadFromJsonAsync<List<CurrentChallengeResponse>>();
					}
					return Page();
				}
				else
				{
					return Page();
				}
			}
		}

		public async Task<IActionResult> OnPostAcceptAsync(int challengeId)
		{
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				var apiUrl = $"{_config["ApiUrl"]}/Challenge/Accept/{challengeId}";
				var response = await httpClient.PutAsync(apiUrl, null);

				if (response.IsSuccessStatusCode)
				{
					return RedirectToPage();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to update challenge: {response.StatusCode}");
					return Page();
				}
			}
		}

		public async Task<IActionResult> OnPostDeclineAsync(int challengeId)
		{
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				var apiUrl = $"{_config["ApiUrl"]}/Challenge/Decline/{challengeId}";
				var response = await httpClient.PutAsync(apiUrl, null);

				if (response.IsSuccessStatusCode)
				{
					return RedirectToPage();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to update challenge: {response.StatusCode}");
					return Page();
				}
			}
		}
	}
}
