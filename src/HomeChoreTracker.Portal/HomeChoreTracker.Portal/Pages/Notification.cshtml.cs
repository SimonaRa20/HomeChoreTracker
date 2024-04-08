using HomeChoreTracker.Portal.Models.Gamification;
using HomeChoreTracker.Portal.Models.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages
{
    public class NotificationModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<NotificationResponse> Notifications { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

        public NotificationModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
                var apiUrl = $"{_config["ApiUrl"]}/Notification/all/skip{skip}/take{pageSize}";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Notifications = await response.Content.ReadFromJsonAsync<List<NotificationResponse>>();

                    var apiUrlAll = _config["ApiUrl"] + $"/Notification/all";
                    var totalCountResponse = await httpClient.GetAsync(apiUrlAll);
                    if (totalCountResponse.IsSuccessStatusCode)
                    {
                        List<NotificationResponse> list = await totalCountResponse.Content.ReadFromJsonAsync<List<NotificationResponse>>();
                        Count = list.Count;
                        TotalPages = (int)Math.Ceiling((double)Count / pageSize);
                    }
                    return Page();
                }
                else
                {
                    return Page();
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_config["ApiUrl"]}/Notification";
                var response = await httpClient.PutAsync(apiUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    return Redirect("/Notification");
                }
                else
                {
                    return Page();
                }
            }
        }
    }
}
