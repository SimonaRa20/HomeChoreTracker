using HomeChoreTracker.Portal.Constants;
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
        public List<TaskAssignmentResponse> HomeChoreResponse { get; set; }

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

                    var apiUrlHomeChore = $"{_config["ApiUrl"]}/Calendar/Chores";

                    var responseHomeChore = await httpClient.GetAsync(apiUrlHomeChore);

                    if (response.IsSuccessStatusCode)
                    {
                        HomeChoreResponse = await responseHomeChore.Content.ReadFromJsonAsync<List<TaskAssignmentResponse>>();

                        var eventsHomeChore = HomeChoreResponse.Select(chore => new
                        {
                            id = chore.Id,
                            title = chore.Task.Name,
                            start = chore.StartDate.ToString("yyyy-MM-dd"),
                            end = chore.EndDate.ToString("yyyy-MM-dd"),
                            assigned = chore.HomeMemberId,
                            description = chore.Task.Description,
                            type = chore.Task.ChoreType.ToString(),
                            time = chore.Task.Time.ToString(),
                        });

                        
                    }
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

        public async Task<IActionResult> OnGetDetailAsync()
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/Calendar/Chores/File";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var calendarFileBytes = await response.Content.ReadAsByteArrayAsync();
                    var contentType = "text/calendar"; // Set the content type to indicate that this is a calendar file
                    var fileName = "HomeChores.ics"; // Set the file name

                    // Return the calendar file as a FileResult
                    return File(calendarFileBytes, contentType, fileName);
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }

    }
}
