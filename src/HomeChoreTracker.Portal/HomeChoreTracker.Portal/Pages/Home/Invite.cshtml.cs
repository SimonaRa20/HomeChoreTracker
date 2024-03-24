using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Home
{
    public class InviteModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		[BindProperty]
		public int Id { get; set; }

		[BindProperty]
		public InviteUserRequest InviteUserRequest { get; set; }

		public InviteModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
		}

		public void OnGet(int id)
		{
			Id = id;
		}


		public async Task<IActionResult> OnPostGenerateInvitationAsync()
		{
			if (!ModelState.IsValid)
			{
				OnGet(Id);
				return Page();
			}

			try
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
					var apiUrl = _config["ApiUrl"] + "/Home/GenerateInvitation";
					InviteUserRequest.HomeId = Id;
					var response = await httpClient.PostAsJsonAsync(apiUrl, InviteUserRequest);

					if (response.IsSuccessStatusCode)
					{

						return RedirectToAction("Index");
					}
					else
					{
						var errorMessage = await response.Content.ReadAsStringAsync();
						ModelState.AddModelError(string.Empty, errorMessage);
						return Page();
					}
				}
			}
			catch (Exception ex)
			{
				OnGet(Id);
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				return Page();
			}
		}
	}
}
