using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace HomeChoreTracker.Portal.Pages.HomeChores
{
    public class AddHomeChoreFromDefaultListModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

		[BindProperty]
		public int HomeId { get; set; }

		[BindProperty(SupportsGet = true)]
		public int CurrentPage { get; set; } = 1;

		public int Count { get; set; }
		public int PageSize { get; set; } = 5;
		public int TotalPages { get; set; }
		public bool ShowPrevious => CurrentPage > 1;
		public bool ShowNext => CurrentPage < TotalPages;
		public List<HomeChoreBaseResponse> HomeChoreBases { get; set; }
		public bool Unauthorized { get; set; }

		public AddHomeChoreFromDefaultListModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            HomeId = id;
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                int pageSize = PageSize;
                int skip = (CurrentPage - 1) * pageSize;
                var apiUrl = $"{_config["ApiUrl"]}/HomeChoreBase/skip{skip}/take{pageSize}";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    HomeChoreBases = await response.Content.ReadFromJsonAsync<List<HomeChoreBaseResponse>>();
                    var apiUrlAll = _config["ApiUrl"] + "/HomeChoreBase";
                    var totalCountResponse = await httpClient.GetAsync(apiUrlAll);

                    if (totalCountResponse.IsSuccessStatusCode)
                    {
                        List<HomeChoreBaseResponse> list = await totalCountResponse.Content.ReadFromJsonAsync<List<HomeChoreBaseResponse>>();
                        Count = list.Count;
                        TotalPages = (int)Math.Ceiling((double)Count / pageSize);
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

        public async Task<IActionResult> OnPostAddAsync(int taskId)
        {
            var token = User.FindFirstValue("Token");

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_config["ApiUrl"]}/HomeChore/{HomeId}?taskId={taskId}";
                var response = await httpClient.PostAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to add chore: {response.StatusCode}");
                    return Page();
                }
            }
        }
    }
}
