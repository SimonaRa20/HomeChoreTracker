using HomeChoreTracker.Portal.Models.Calendar;
using HomeChoreTracker.Portal.Models.HomeChore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        public CalendarRequest CalendarRequest { get; set; }

        public List<Event> Events { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
            CalendarRequest = new CalendarRequest();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = User.FindFirstValue("Token");

            if(token == null)
            {
                return RedirectToPage("Auth/Login");
            }
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/Calendar";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Events = await response.Content.ReadFromJsonAsync<List<Event>>();

                    var events = Events.Select(chore => new
                    {
                        id = chore.Id,
                        start = chore.StartDate.ToString("yyyy-MM-dd"),
                        end = chore.EndDate.ToString("yyyy-MM-dd"),
                        title = chore.Summary,
                    });

                    return Page();
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await HttpContext.SignOutAsync();
                        return RedirectToPage("/Index");
                    }

                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = User.FindFirstValue("Token");
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = $"{_config["ApiUrl"]}/Calendar";

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(new StreamContent(CalendarRequest.File.OpenReadStream()), "File", CalendarRequest.File.FileName);

                        var response = await httpClient.PostAsync(apiUrl, formData);

                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToPage();
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            ModelState.AddModelError(string.Empty, $"Failed to add calendar events: {errorContent}");
                            return Page();
                        }
                    }
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
