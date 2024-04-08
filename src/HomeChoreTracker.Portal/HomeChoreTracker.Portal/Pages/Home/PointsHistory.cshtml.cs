using HomeChoreTracker.Portal.Models.Gamification;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Home
{
    public class PointsHistoryModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public List<PointsResponse> History { get; set; }

		[BindProperty(SupportsGet = true)]
		public int CurrentPage { get; set; } = 1;
		public int Count { get; set; }
		public int PageSize { get; set; } = 10;
		public int TotalPages { get; set; }

		public bool ShowPrevious => CurrentPage > 1;
		public bool ShowNext => CurrentPage < TotalPages;
		public int Id { get; set; }

		public PointsHistoryModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

				int pageSize = PageSize;
				int skip = (CurrentPage - 1) * pageSize;

				var apiUrl = $"{_config["ApiUrl"]}/Home/{id}/skip{skip}/take{pageSize}";

				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					History = await response.Content.ReadFromJsonAsync<List<PointsResponse>>();
                    var apiUrlAll = _config["ApiUrl"] + $"/Home/{id}";
                    var totalCountResponse = await httpClient.GetAsync(apiUrlAll);
                    if (totalCountResponse.IsSuccessStatusCode)
                    {
                        List<PointsResponse> list = await totalCountResponse.Content.ReadFromJsonAsync<List<PointsResponse>>();
                        Count = list.Count;
                        TotalPages = (int)Math.Ceiling((double)Count / pageSize);
                    }

                    return Page();
                }
				else
				{
					return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
				}
			}
		}
    }
}
