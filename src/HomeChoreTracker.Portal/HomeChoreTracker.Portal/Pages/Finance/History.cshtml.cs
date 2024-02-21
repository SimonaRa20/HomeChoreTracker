using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using HomeChoreTracker.Portal.Models.Finance;
using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeChoreTracker.Portal.Pages.Finance
{
    public class HistoryModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public List<TransferHistoryItem> TransferHistory { get; set; }

        public HistoryModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
			_config = config;
        }

		public async Task<IActionResult> OnGetAsync()
		{
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var apiUrl = $"{_config["ApiUrl"]}/Finance/transferHistory";

				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					TransferHistory = await response.Content.ReadFromJsonAsync<List<TransferHistoryItem>>();
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
