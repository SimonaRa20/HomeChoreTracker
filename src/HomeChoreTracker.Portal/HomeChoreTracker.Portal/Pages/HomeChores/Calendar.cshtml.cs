using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HomeChoreTracker.Portal.Models.HomeChore;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.HomeChores
{
    public class CalendarModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<HomeChoreResponse> HomeChoreResponse { get; set; }

        public CalendarModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        [BindProperty]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/HomeChore/{id}";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    HomeChoreResponse = await response.Content.ReadFromJsonAsync<List<HomeChoreResponse>>();

                    var events = HomeChoreResponse.Select(chore => new
                    {
                        id = chore.Id,
                        title = chore.Name,
                        start = chore.StartDate.ToString("yyyy-MM-dd"),
                        end = chore.EndDate != null ? chore.EndDate.Value.ToString("yyyy-MM-dd") : chore.StartDate.AddDays(1).ToString("yyyy-MM-dd")
                    });

                    return Page(); // Return events as JSON result
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }
    }
}
