using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Finance
{
    public class CreateIncomeModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		[BindProperty]
		public HomeChoreBaseCreateRequest CreateHomeChoreBase { get; set; }

		public CreateIncomeModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
			CreateHomeChoreBase = new HomeChoreBaseCreateRequest();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ClearFieldErrors(key => key == "CreateHomeChoreBase.ChoreType");
			ClearFieldErrors(key => key == "CreateHomeChoreBase.Frequency");

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
					var apiUrl = _config["ApiUrl"] + "/HomeChoreBase";

					var response = await httpClient.PostAsJsonAsync(apiUrl, CreateHomeChoreBase);

					if (response.IsSuccessStatusCode)
					{
						return RedirectToPage("/HomeChoreBase/Index");
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

		private void ClearFieldErrors(Func<string, bool> predicate)
		{
			foreach (var field in ModelState)
			{
				if (field.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
				{
					if (predicate(field.Key))
					{
						field.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
					}
				}
			}
		}
	}
}
