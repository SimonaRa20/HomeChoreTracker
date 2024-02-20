using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Homes
{
    public class InviteToHomeModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		[BindProperty]
		public InviteUserRequest InviteUserRequest { get; set; }

		public List<HomeResponse> Homes { get; set; }

		public InviteToHomeModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

				var apiUrl = $"{_config["ApiUrl"]}/Home";

				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					Homes = await response.Content.ReadFromJsonAsync<List<HomeResponse>>();
					return Page();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {response.ReasonPhrase}");
					return Page();
				}
			}
		}


		public async Task<IActionResult> OnPostGenerateInvitationAsync()
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
					var apiUrl = _config["ApiUrl"] + "/Home/GenerateInvitation";

					var response = await httpClient.PostAsJsonAsync(apiUrl, InviteUserRequest);

					if (response.IsSuccessStatusCode)
					{

						return RedirectToAction("Index");
					}
					else
					{
						await OnGetAsync();
						ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
						return Page();
					}
				}
			}
			catch (Exception ex)
			{
				await OnGetAsync();
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				return Page();
			}
		}

	}
}
