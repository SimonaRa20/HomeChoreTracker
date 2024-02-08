using HomeChoreTracker.Portal.Models.Home;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeChoreTracker.Portal.Pages.HomeChoreBase
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<HomeChoreBaseResponse> HomeChoreBases { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }

        [BindProperty]
        public int SelectedDelete { get; set; }

        [BindProperty]
        public HomeChoreBaseRequest CreateHomeChoreBase { get; set; }

        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

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
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            try
            {
                var token = User.FindFirstValue("Token");
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = _config["ApiUrl"] + "/HomeChoreBase";

                    var response = await httpClient.PostAsJsonAsync(apiUrl, CreateHomeChoreBase);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
                        await OnGetAsync();
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_config["ApiUrl"]}/HomeChoreBase/{id}";

                var response = await httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    // Handle deletion failure
                    ModelState.AddModelError(string.Empty, $"Failed to delete chore: {response.StatusCode}");
                    return Page();
                }
            }
        }
    }
}
