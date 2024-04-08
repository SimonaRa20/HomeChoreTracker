using HomeChoreTracker.Portal.Models.Forum;
using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Forum
{
    public class CreateModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		[BindProperty]
		public AdviceRequest CreateAdvice { get; set; }

		public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
		}

		public async Task<IActionResult> OnPostAsync()
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
					var apiUrl = _config["ApiUrl"] + "/Forum";
					var response = await httpClient.PostAsJsonAsync(apiUrl, CreateAdvice);

					if (response.IsSuccessStatusCode)
					{
						return RedirectToPage("/Forum/Index");
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
