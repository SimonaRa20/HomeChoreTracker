using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using HomeChoreTracker.Portal.Constants;
using HomeChoreTracker.Portal.Models.Home;
using HomeChoreTracker.Portal.Models.HomeChore;
using HomeChoreTracker.Portal.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using DayOfWeek = HomeChoreTracker.Portal.Constants.DayOfWeek;

namespace HomeChoreTracker.Portal.Pages.HomeChores
{
    public class CalendarModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<TaskAssignmentResponse> HomeChoreResponse { get; set; }
        public List<UserGetResponse> HomeMembers { get; set; }
        public CalendarModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public AssignedHomeMember AssignedHomeMember { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/HomeChore/Chore/Calendar/{id}";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    HomeChoreResponse = await response.Content.ReadFromJsonAsync<List<TaskAssignmentResponse>>();

                    var events = HomeChoreResponse.Select(chore => new
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


                    var apiUrlMembers = $"{_config["ApiUrl"]}/Home/HomeMembers?homeId={Id}";

                    var responseMembers = await httpClient.GetAsync(apiUrlMembers);

                    if (responseMembers.IsSuccessStatusCode)
                    {
                        HomeMembers = await responseMembers.Content.ReadFromJsonAsync<List<UserGetResponse>>();
                    }
                    return Page();
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(int homeId, int taskId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = _config["ApiUrl"] + $"/Calendar/{homeId}";

                var selectedMemberId = int.Parse(Request.Form["AssignedHomeMember.HomeMemberId"]);

                AssignedHomeMember = new AssignedHomeMember
                {
                    TaskId = taskId,
                    HomeMemberId = selectedMemberId
                };

                var response = await httpClient.PutAsJsonAsync(apiUrl, AssignedHomeMember);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/HomeChores/Calendar", new { id = homeId });
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return Page();
                }

            }
        }
    }
}