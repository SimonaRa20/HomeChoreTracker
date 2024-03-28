using HomeChoreTracker.Portal.Models.Finance;
using HomeChoreTracker.Portal.Models.Home;
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
		public IncomeRequest CreateNewIncome { get; set; }

		public List<HomeResponse> Homes { get; set; }
		public List<FinancialCategory> FinancialCategories { get; set; }

		public CreateIncomeModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
			CreateNewIncome = new IncomeRequest();
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
					var apiUrlCategories = $"{_config["ApiUrl"]}/Finance/CategoriesIncome";

					var responseCategories = await httpClient.GetAsync(apiUrlCategories);

					if (responseCategories.IsSuccessStatusCode)
					{
						FinancialCategories = await responseCategories.Content.ReadFromJsonAsync<List<FinancialCategory>>();
						return Page();
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {response.ReasonPhrase}");
						return Page();
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {response.ReasonPhrase}");
					return Page();
				}
			}
		}

		public async Task<IActionResult> OnPostAsync()
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
					var apiUrl = _config["ApiUrl"] + "/Finance/income";

					var response = await httpClient.PostAsJsonAsync(apiUrl, CreateNewIncome);

					if (response.IsSuccessStatusCode)
					{
						await OnGetAsync();
						return RedirectToPage("/Finance/Index");
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
						await OnGetAsync();
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
