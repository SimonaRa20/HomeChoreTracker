using HomeChoreTracker.Portal.Models.Profile;
using HomeChoreTracker.Portal.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Profile
{
    public class BusyIntervalsModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		[BindProperty]
		public BusyIntervalRequest CreateBusyInterval { get; set; }

		[BindProperty]
		public BusyIntervalRequest EditBusyInterval { get; set; }

		public List<BusyIntervalResponse> BusyIntervals { get; set; }

		public BusyIntervalsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			if (User.Identity.IsAuthenticated)
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
					var apiUrlInterval = _config["ApiUrl"] + "/User/BusyIntervals";
					var response = await httpClient.GetAsync(apiUrlInterval);

					if (response.IsSuccessStatusCode)
					{
						BusyIntervals = await response.Content.ReadFromJsonAsync<List<BusyIntervalResponse>>();
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
					}
					return Page();
				}
			}
			return Redirect("/");
		}

		public async Task<IActionResult> OnPostNewBusyIntervalAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			try
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
					var apiUrl = _config["ApiUrl"] + "/User/BusyInterval";

					var response = await httpClient.PostAsJsonAsync(apiUrl, CreateBusyInterval);

					if (response.IsSuccessStatusCode)
					{
						return await OnGetAsync();
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
						return Page();
					}
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				return Page();
			}
		}

		public async Task<IActionResult> OnPostDeleteAsync(int id)
		{
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				var apiUrl = $"{_config["ApiUrl"]}/User/BusyInterval/{id}";

				var response = await httpClient.DeleteAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					return RedirectToPage();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to delete chore: {response.StatusCode}");
					return Page();
				}
			}
		}

		public async Task<IActionResult> OnPostEditBusyIntervalsAsync(int intervalId)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			try
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
					var apiUrl = _config["ApiUrl"] + $"/User/BusyInterval/{intervalId}";

					EditBusyInterval.Day = Enum.Parse<System.DayOfWeek>(Request.Form["EditDay"]);
					EditBusyInterval.StartTime = TimeSpan.Parse(Request.Form["EditStartTime"]);
					EditBusyInterval.EndTime = TimeSpan.Parse(Request.Form["EditEndTime"]);

					var response = await httpClient.PutAsJsonAsync(apiUrl, EditBusyInterval);

					if (response.IsSuccessStatusCode)
					{
						return await OnGetAsync();
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
						return Page();
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
