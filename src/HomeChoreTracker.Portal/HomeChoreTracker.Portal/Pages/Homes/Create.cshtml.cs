using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Homes
{
    public class CreateModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		[BindProperty]
		public HomeRequest CreateHome { get; set; }

		public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
		}

		public async Task<IActionResult> OnPostCreateHomeAsync()
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
					var apiUrl = _config["ApiUrl"] + "/Home";
					var response = await httpClient.PostAsJsonAsync(apiUrl, CreateHome);

					if (response.IsSuccessStatusCode)
					{
						return RedirectToPage("/Homes/Index");
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
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				return Page();
			}
		}
	}
}
