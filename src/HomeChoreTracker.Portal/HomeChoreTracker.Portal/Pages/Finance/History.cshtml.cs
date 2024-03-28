using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using HomeChoreTracker.Portal.Constants;
using HomeChoreTracker.Portal.Models.Finance;
using HomeChoreTracker.Portal.Models.Home;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeChoreTracker.Portal.Pages.Finance
{
    public class HistoryModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public List<TransferHistoryItem> TransferHistory { get; set; }

        public List<HomeResponse> Homes { get; set; }
		public List<FinancialCategory> FinancialCategoriesIncome { get; set; }
		public List<FinancialCategory> FinancialCategoriesExpense { get; set; }
		[BindProperty]
        public string DeleteItemType { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }

        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

		[BindProperty]
		public IncomeRequest EditIncome { get; set; }

		[BindProperty]
		public ExpenseRequest EditExpense { get; set; }

		public HistoryModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
			_config = config;
            EditExpense = new ExpenseRequest();
            EditIncome = new IncomeRequest();
        }

		public async Task<IActionResult> OnGetAsync()
		{
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var apiUrlHome = $"{_config["ApiUrl"]}/Home";

				var responseHomes = await httpClient.GetAsync(apiUrlHome);

				if (responseHomes.IsSuccessStatusCode)
				{
					Homes = await responseHomes.Content.ReadFromJsonAsync<List<HomeResponse>>();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {responseHomes.ReasonPhrase}");
				}

				int pageSize = PageSize;
                int skip = (CurrentPage - 1) * pageSize;

                var apiUrl = $"{_config["ApiUrl"]}/Finance/transferHistory/skip{skip}/take{pageSize}";

				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					TransferHistory = await response.Content.ReadFromJsonAsync<List<TransferHistoryItem>>();
                    var apiUrlAll = _config["ApiUrl"] + "/Finance/transferHistory";
                    var totalCountResponse = await httpClient.GetAsync(apiUrlAll);
                    if (totalCountResponse.IsSuccessStatusCode)
                    {
                        List<TransferHistoryItem> list = await totalCountResponse.Content.ReadFromJsonAsync<List<TransferHistoryItem>>();
                        Count = list.Count;
                        TotalPages = (int)Math.Ceiling((double)Count / pageSize);
                    }

					var apiUrlICategories = $"{_config["ApiUrl"]}/Finance/CategoriesIncome";

					var responseICategories = await httpClient.GetAsync(apiUrlICategories);

					if (responseICategories.IsSuccessStatusCode)
					{
						FinancialCategoriesIncome = await responseICategories.Content.ReadFromJsonAsync<List<FinancialCategory>>();
					}

					var apiUrlECategories = $"{_config["ApiUrl"]}/Finance/CategoriesExpense";

					var responseECategories = await httpClient.GetAsync(apiUrlECategories);

					if (responseECategories.IsSuccessStatusCode)
					{
						FinancialCategoriesExpense = await responseECategories.Content.ReadFromJsonAsync<List<FinancialCategory>>();
					}

					return Page();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {response.ReasonPhrase}");
					return Page();
				}
			}
        }

		public async Task<IActionResult> OnPostEditIncomeAsync(int id)
		{
			ClearFieldErrors(key => key == "Title");
			ClearFieldErrors(key => key == "DeleteItemType");
			if (!ModelState.IsValid)
			{
				await OnGetAsync(); // Refresh the data
				return Page();
			}

			try
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

					var apiUrlHome = $"{_config["ApiUrl"]}/Home";

					var responseHomes = await httpClient.GetAsync(apiUrlHome);

					if (responseHomes.IsSuccessStatusCode)
					{
						Homes = await responseHomes.Content.ReadFromJsonAsync<List<HomeResponse>>();
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {responseHomes.ReasonPhrase}");
					}

					var apiUrl = $"{_config["ApiUrl"]}/Finance/income/{id}";

					// Update EditHomeChore with form values
					EditIncome.Title = Request.Form["EditIncomeTitle"];
					EditIncome.Amount = decimal.Parse(Request.Form["EditIncomeAmount"]);
					EditIncome.Description = Request.Form["EditIncomeDescription"];
					EditIncome.Time = DateTime.Parse(Request.Form["EditIncomeTime"]);
					if (!string.IsNullOrEmpty(Request.Form["EditIncomeType"]))
					{
						if (int.TryParse(Request.Form["EditIncomeType"], out int editIncomeCategoryId))
						{
							if(editIncomeCategoryId == 0)
							{
								EditIncome.NewFinancialCategory = Request.Form["NewIncomeFinancialCategory"];
								EditIncome.FinancialCategoryId = 0;
							}
							else
							{
								EditIncome.FinancialCategoryId = editIncomeCategoryId;
							}
						}
					}
					if (!string.IsNullOrEmpty(Request.Form["EditIncomeHome"]))
					{
						if (int.TryParse(Request.Form["EditIncomeHome"], out int editIncomeHomeId))
						{
							EditIncome.HomeId = editIncomeHomeId;
						}
					}

					var response = await httpClient.PutAsJsonAsync(apiUrl, EditIncome);

					if (response.IsSuccessStatusCode)
					{
						return RedirectToPage();
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Failed to update chore: {response.StatusCode}");
						return Page();
					}
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				await OnGetAsync(); // Refresh the data
				return Page();
			}
		}

		public async Task<IActionResult> OnPostEditExpenseAsync(int id)
		{
			ClearFieldErrors(key => key == "Title");
			ClearFieldErrors(key => key == "DeleteItemType");
			if (!ModelState.IsValid)
			{
				await OnGetAsync(); // Refresh the data
				return Page();
			}

			try
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

					var apiUrlHome = $"{_config["ApiUrl"]}/Home";

					var responseHomes = await httpClient.GetAsync(apiUrlHome);

					if (responseHomes.IsSuccessStatusCode)
					{
						Homes = await responseHomes.Content.ReadFromJsonAsync<List<HomeResponse>>();
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {responseHomes.ReasonPhrase}");
					}

					var apiUrl = $"{_config["ApiUrl"]}/Finance/expense/{id}";

					// Update EditHomeChore with form values
					EditExpense.Title = Request.Form["EditExpenseTitle"];
					EditExpense.Amount = decimal.Parse(Request.Form["EditExpenseAmount"]);
					EditExpense.Description = Request.Form["EditExpenseDescription"];
					EditExpense.Time = DateTime.Parse(Request.Form["EditExpenseTime"]);
					if (!string.IsNullOrEmpty(Request.Form["EditExpenseType"]))
					{
						if (int.TryParse(Request.Form["EditExpenseType"], out int editExpenseTypeId))
						{
							if (editExpenseTypeId == 0)
							{
								EditExpense.NewFinancialCategory = Request.Form["NewExpenseFinancialCategory"];
								EditExpense.FinancialCategoryId = 0;
							}
							else
							{
								EditExpense.FinancialCategoryId = editExpenseTypeId;
							}
						}
					}
					if (!string.IsNullOrEmpty(Request.Form["EditExpenseHome"]))
					{
						if (int.TryParse(Request.Form["EditExpenseHome"], out int editExpenseId))
						{
							EditExpense.HomeId = editExpenseId;
						}
					}

					var response = await httpClient.PutAsJsonAsync(apiUrl, EditExpense);

					if (response.IsSuccessStatusCode)
					{
						return RedirectToPage();
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Failed to update chore: {response.StatusCode}");
						return Page();
					}
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				await OnGetAsync(); // Refresh the data
				return Page();
			}
		}


		public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                DeleteItemType = Request.Form["DeleteItemType"];
                var apiUrl = $"{_config["ApiUrl"]}/Finance/{id}?type={DeleteItemType}";


                var response = await httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    await OnGetAsync();
                    return RedirectToPage();
                }
                else
                {
                    // Handle deletion failure
                    await OnGetAsync();
                    ModelState.AddModelError(string.Empty, $"Failed to delete chore: {response.StatusCode}");
                    return Page();
                }
            }
        }

		public async Task<IActionResult> OnGetIncomeDetailAsync(int id)
		{
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var apiUrlHome = $"{_config["ApiUrl"]}/Home";

				var responseHomes = await httpClient.GetAsync(apiUrlHome);

				if (responseHomes.IsSuccessStatusCode)
				{
					Homes = await responseHomes.Content.ReadFromJsonAsync<List<HomeResponse>>();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {responseHomes.ReasonPhrase}");
				}


				var apiUrl = $"{_config["ApiUrl"]}/Finance/income/{id}";
				var response = await httpClient.GetAsync(apiUrl);

				var incomeDetails = new IncomeResponse();
				if (response.IsSuccessStatusCode)
				{
					incomeDetails = await response.Content.ReadFromJsonAsync<IncomeResponse>();
					var home = Homes.FirstOrDefault(x => x.Id.Equals(incomeDetails.HomeId));
					if (incomeDetails != null)
					{
						string descriptionText = incomeDetails.Description ?? "-";
						return new JsonResult(new
						{
							title = incomeDetails.Title,
							amount = incomeDetails.Amount,
							description = descriptionText,
							time = incomeDetails.Time,
							type = incomeDetails.Type.ToString(),
							financialCategory = incomeDetails.FinancialCategory,
							financialCategoryId = incomeDetails.FinancialCategoryId,
							home = home?.Title ?? "-", // Include home title in the response
							homeId = home?.Id,
						});
					}
					else
					{
						return NotFound("Chore details not found.");
					}
				}
				else
				{
					return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
				}
			}
		}


		public async Task<IActionResult> OnGetExpenseDetailAsync(int id)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var apiUrlHome = $"{_config["ApiUrl"]}/Home";

				var responseHomes = await httpClient.GetAsync(apiUrlHome);

				if (responseHomes.IsSuccessStatusCode)
				{
					Homes = await responseHomes.Content.ReadFromJsonAsync<List<HomeResponse>>();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {responseHomes.ReasonPhrase}");
				}

				var apiUrl = $"{_config["ApiUrl"]}/Finance/expense/{id}";
                var response = await httpClient.GetAsync(apiUrl);

                var expenseDetails = new ExpenseResponse();
                if (response.IsSuccessStatusCode)
                {
                    expenseDetails = await response.Content.ReadFromJsonAsync<ExpenseResponse>();
					var home = Homes.FirstOrDefault(x => x.Id.Equals(expenseDetails.HomeId));
					if (expenseDetails != null)
                    {
                        string descriptionText = expenseDetails.Description ?? "-";
                        return new JsonResult(new
                        {
                            title = expenseDetails.Title,
                            amount = expenseDetails.Amount,
                            description = descriptionText,
                            time = expenseDetails.Time,
                            type = expenseDetails.Type.ToString(),
							financialCategory = expenseDetails.FinancialCategory,
							financialCategoryId = expenseDetails.FinancialCategoryId,
							home = home?.Title ?? "-",
							homeId = home?.Id
                        });
                    }
                    else
                    {
                        return NotFound("Chore details not found.");
                    }
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }
		public async Task<IActionResult> OnGetEditIncomeAsync(int id)
		{
			if (!ModelState.IsValid)
			{
				await OnGetAsync(); // Refresh the data
				return Page();
			}

			try
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

					var apiUrl = $"{_config["ApiUrl"]}/Finance/income/{id}";

					var response = await httpClient.GetAsync(apiUrl);

					if (response.IsSuccessStatusCode)
					{
						EditIncome = await response.Content.ReadFromJsonAsync<IncomeRequest>();
						// Fetch homes
						var homesResponse = await httpClient.GetAsync($"{_config["ApiUrl"]}/Home");
						if (homesResponse.IsSuccessStatusCode)
						{
							Homes = await homesResponse.Content.ReadFromJsonAsync<List<HomeResponse>>();
							return Page();
						}
						else
						{
							ModelState.AddModelError(string.Empty, $"Failed to retrieve homes: {homesResponse.ReasonPhrase}");
						}

						return Page();
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Failed to retrieve income: {response.ReasonPhrase}");
						return Page();
					}
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				await OnGetAsync(); // Refresh the data
				return Page();
			}
		}

		public async Task<IActionResult> OnGetEditExpenseAsync(int id)
		{
			if (!ModelState.IsValid)
			{
				await OnGetAsync(); // Refresh the data
				return Page();
			}

			try
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

					var apiUrl = $"{_config["ApiUrl"]}/Finance/expense/{id}";

					var response = await httpClient.GetAsync(apiUrl);

					if (response.IsSuccessStatusCode)
					{
						EditExpense = await response.Content.ReadFromJsonAsync<ExpenseRequest>();
						// Fetch homes
						var homesResponse = await httpClient.GetAsync($"{_config["ApiUrl"]}/Home");
						if (homesResponse.IsSuccessStatusCode)
						{
							Homes = await homesResponse.Content.ReadFromJsonAsync<List<HomeResponse>>();
							return Page();
						}
						else
						{
							ModelState.AddModelError(string.Empty, $"Failed to retrieve homes: {homesResponse.ReasonPhrase}");
						}

						return Page();
					}
					else
					{
						ModelState.AddModelError(string.Empty, $"Failed to retrieve income: {response.ReasonPhrase}");
						return Page();
					}
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				await OnGetAsync(); // Refresh the data
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
