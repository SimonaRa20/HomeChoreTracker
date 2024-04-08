using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

		[BindProperty]
		public int Id { get; set; }

		[BindProperty]
		public int UserId { get; set; }

		[BindProperty]
		public AssignedHomeMember AssignedHomeMember { get; set; }

		public List<TaskAssignmentResponse> HomeChoreResponse { get; set; } = new List<TaskAssignmentResponse> { };
        public List<UserGetResponse> HomeMembers { get; set; } = new List<UserGetResponse> { };
		public bool Unauthorized { get; set; }

		public CalendarModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
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
                        isDone = chore.IsDone,
                        votes = chore.TotalVotes,
                    });

                    var apiUrlMembers = $"{_config["ApiUrl"]}/Home/HomeMembers?homeId={Id}";
                    var responseMembers = await httpClient.GetAsync(apiUrlMembers);

                    if (responseMembers.IsSuccessStatusCode)
                    {
                        HomeMembers = await responseMembers.Content.ReadFromJsonAsync<List<UserGetResponse>>();
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
                var selectedMemberId = Request.Form["AssignedHomeMember.HomeMemberId"];

                if (string.IsNullOrEmpty(selectedMemberId))
                {
                    
                    AssignedHomeMember = new AssignedHomeMember
                    {
                        TaskId = taskId,
                        HomeMemberId = null,
                    };
                }
                else
                {
                    AssignedHomeMember = new AssignedHomeMember
                    {
                        TaskId = taskId,
                        HomeMemberId = int.Parse(selectedMemberId)
                    };
                }

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

        public async Task<IActionResult> OnPostVoteAsync(int taskId, int vote, int homeId)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var voteApiUrl = $"{_config["ApiUrl"]}/HomeChore/VoteTask/{taskId}/{vote}";
                    var response = await httpClient.PutAsync(voteApiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToPage("/HomeChores/Calendar", new { id = homeId });
                    }
                    else
                    {
                        return BadRequest($"Failed to vote: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"An error occurred: {ex.Message}");
                }
            }
        }

        public async Task<IActionResult> OnPostSystemAssignAsync(int homeId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = _config["ApiUrl"] + $"/Calendar/AssignTasksToMembers/{homeId}";
                var emptyContent = new StringContent("");
                var response = await httpClient.PostAsync(apiUrl, emptyContent);

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