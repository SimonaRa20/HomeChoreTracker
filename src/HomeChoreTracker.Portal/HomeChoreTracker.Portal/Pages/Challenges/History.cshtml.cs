using HomeChoreTracker.Portal.Models.Challenge;
using HomeChoreTracker.Portal.Models.HomeChore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Challenges
{
    public class HistoryModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public List<HistoryChallengeResponse> HistoryChallengeResponses { get; set; }

		[BindProperty(SupportsGet = true)]
		public int CurrentPage { get; set; } = 1;

		public int Count { get; set; }
		public int PageSize { get; set; } = 8;
		public int TotalPages { get; set; }
		public bool ShowPrevious => CurrentPage > 1;
		public bool ShowNext => CurrentPage < TotalPages;

		public HistoryModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

				var apiUrl = $"{_config["ApiUrl"]}/Challenge/HistoryChallenges";

				var response = await httpClient.GetAsync(apiUrl);
				int pageSize = PageSize;
				int skip = (CurrentPage - 1) * pageSize;

				if (response.IsSuccessStatusCode)
				{
					List<HistoryChallengeResponse> list = await response.Content.ReadFromJsonAsync<List<HistoryChallengeResponse>>();
					Count = list.Count;
					TotalPages = (int)Math.Ceiling((double)Count / pageSize);
					HistoryChallengeResponses = list.Skip(skip).Take(pageSize).ToList();

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
