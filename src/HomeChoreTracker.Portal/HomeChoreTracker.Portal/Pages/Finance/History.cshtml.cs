using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using HomeChoreTracker.Portal.Models.Finance;
using HomeChoreTracker.Portal.Models.Home;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeChoreTracker.Portal.Pages.Finance
{
    public class HistoryModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public List<TransferHistoryItem> TransferHistory { get; set; }
        [BindProperty]
        public string DeleteItemType { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }

        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

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

                int pageSize = PageSize;
                int skip = (CurrentPage - 1) * pageSize;

                var apiUrl = $"{_config["ApiUrl"]}/Finance/transferHistory/skip{skip}/take{pageSize}";

				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					TransferHistory = await response.Content.ReadFromJsonAsync<List<TransferHistoryItem>>();
                    var apiUrlAll = _config["ApiUrl"] + "/Finance/transferHistory";
                    var totalCountResponse = await httpClient.GetAsync(apiUrlAll);
                    if (totalCountResponse.IsSuccessStatusCode)
                    {
                        List<TransferHistoryItem> list = await totalCountResponse.Content.ReadFromJsonAsync<List<TransferHistoryItem>>();
                        Count = list.Count;
                        TotalPages = (int)Math.Ceiling((double)Count / pageSize);
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

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                DeleteItemType = Request.Form["DeleteItemType"];
                var apiUrl = $"{_config["ApiUrl"]}/Finance/{id}?type={DeleteItemType}";


                var response = await httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    await OnGetAsync();
                    return RedirectToPage();
                }
                else
                {
                    // Handle deletion failure
                    await OnGetAsync();
                    ModelState.AddModelError(string.Empty, $"Failed to delete chore: {response.StatusCode}");
                    return Page();
                }
            }
        }
    }
}
