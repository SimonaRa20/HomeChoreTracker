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

		public CreateExpenseFromImageModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
			CreateNewExpense = new ExpenseImageRequest();
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

		public async Task<IActionResult> OnPostAsync()
		{
			ClearFieldErrors(key => key == "CreateNewExpense.ExpenseImage");
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

					var apiUrl = _config["ApiUrl"] + "/Finance/expenseimage";

					using (var content = new MultipartFormDataContent())
					{
						content.Add(new StringContent(((int)CreateNewExpense.Type).ToString()), "Type");

						if (CreateNewExpense.HomeId.HasValue)
						{
							content.Add(new StringContent(CreateNewExpense.HomeId.Value.ToString()), "HomeId");
						}

						using (var stream = new MemoryStream())
						{
							CreateNewExpense.ExpenseImage = Request.Form.Files["ExpenseImage"];
							await CreateNewExpense.ExpenseImage.CopyToAsync(stream);
							content.Add(new ByteArrayContent(stream.ToArray()), "ExpenseImage", CreateNewExpense.ExpenseImage.FileName);
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
							return Page();
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
