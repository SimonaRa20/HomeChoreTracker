using HomeChoreTracker.Portal.Models.Finance;
using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Finance
{
	public class CreateExpenseFromImageModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		[BindProperty]
		public ExpenseImageRequest CreateNewExpense { get; set; }

		public List<HomeResponse> Homes { get; set; }
		public List<FinancialCategory> FinancialCategories { get; set; }

		public CreateExpenseFromImageModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
			CreateNewExpense = new ExpenseImageRequest();
		}

		public async Task<IActionResult> OnGetAsync()
		{
			await LoadHomes();
			return Page();
		}

		private async Task LoadHomes()
		{
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				var apiUrl = $"{_config["ApiUrl"]}/Home";
				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					Homes = await response.Content.ReadFromJsonAsync<List<HomeResponse>>();
					var apiUrlCategories = $"{_config["ApiUrl"]}/Finance/CategoriesExpense";
					var responseCategories = await httpClient.GetAsync(apiUrlCategories);

					if (responseCategories.IsSuccessStatusCode)
					{
						FinancialCategories = await responseCategories.Content.ReadFromJsonAsync<List<FinancialCategory>>();
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {response.ReasonPhrase}");
				}
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ClearFieldErrors(key => key == "CreateNewExpense.ExpenseImage");
			if (!ModelState.IsValid)
			{
				return Page();
			}

			try
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
					var apiUrl = $"{_config["ApiUrl"]}/Finance/expenseimage";

					using (var content = new MultipartFormDataContent())
					{
						if (CreateNewExpense.HomeId.HasValue)
						{
							content.Add(new StringContent(CreateNewExpense.HomeId.Value.ToString()), "HomeId");
						}

						content.Add(new StringContent(CreateNewExpense.FinancialCategoryId.ToString()), "FinancialCategoryId");
						if (CreateNewExpense.FinancialCategoryId == 0 && !string.IsNullOrEmpty(CreateNewExpense.NewFinancialCategory))
						{
							content.Add(new StringContent(CreateNewExpense.NewFinancialCategory), "NewFinancialCategory");
						}

						var imageFile = Request.Form.Files["ExpenseImage"];

						if (imageFile == null || imageFile.Length == 0)
						{
							ModelState.AddModelError(string.Empty, string.Empty);
							return await OnGetAsync();
						}

						using (var stream = new MemoryStream())
						{
							await imageFile.CopyToAsync(stream);
							content.Add(new ByteArrayContent(stream.ToArray()), "ExpenseImage", imageFile.FileName);
						}

						var response = await httpClient.PostAsync(apiUrl, content);

						if (response.IsSuccessStatusCode)
						{
							return RedirectToPage("/Finance/Index");
						}
						else
						{
							var errorMessage = await response.Content.ReadAsStringAsync();
							ModelState.AddModelError(string.Empty, errorMessage);
							return await OnGetAsync();
						}
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
