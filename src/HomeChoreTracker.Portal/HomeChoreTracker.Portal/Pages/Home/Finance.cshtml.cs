using HomeChoreTracker.Portal.Constants;
using HomeChoreTracker.Portal.Models.Finance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Home
{
    public class FinanceModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }
        public double CurrentMonthTotalBalance { get; set; }
        public double CurrentMonthTotalIncome { get; set; }
        public double CurrentMonthTotalExpense { get; set; }
		public double CurrentMonthTotalHomeChoresExpense { get; set; }
		public List<MonthlySummary> MonthlySummaries { get; set; }
        public Dictionary<string, decimal> ExpenseCategories { get; set; }
        public Dictionary<string, decimal> IncomeCategories { get; set; }
        public bool Unauthorized { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public FinanceModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            try
            {
                var token = User.FindFirstValue("Token");
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                await GetFinancialSummary(httpClient);
                await GetMonthlySummaries(httpClient);
                await GetExpenseCategories(httpClient);
                await GetIncomeCategories(httpClient);

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {ex.Message}");
                return Page();
            }
        }

        private async Task GetFinancialSummary(HttpClient httpClient)
        {
            var apiUrlBalance = $"{_config["ApiUrl"]}/Finance/totalBalance/{Id}";
            var apiUrlIncome = $"{_config["ApiUrl"]}/Finance/totalIncome/{Id}";
            var apiUrlExpense = $"{_config["ApiUrl"]}/Finance/totalExpense/{Id}";
            var apiUrlHomeChoresExpense = $"{_config["ApiUrl"]}/Finance/totalHomeChores/{Id}";

			CurrentMonthTotalBalance = await GetApiResponse<double>(httpClient, apiUrlBalance);
            CurrentMonthTotalIncome = await GetApiResponse<double>(httpClient, apiUrlIncome);
            CurrentMonthTotalExpense = await GetApiResponse<double>(httpClient, apiUrlExpense);
			CurrentMonthTotalHomeChoresExpense = await GetApiResponse<double>(httpClient, apiUrlHomeChoresExpense);
		}

        private async Task GetMonthlySummaries(HttpClient httpClient)
        {
            var apiUrl = $"{_config["ApiUrl"]}/Finance/monthlySummary/{Id}";
            MonthlySummaries = await GetApiResponse<List<MonthlySummary>>(httpClient, apiUrl);
        }

        private async Task GetExpenseCategories(HttpClient httpClient)
        {
            var apiUrl = $"{_config["ApiUrl"]}/Finance/expenseCategories/{Id}";
            ExpenseCategories = await GetApiResponse<Dictionary<string, decimal>>(httpClient, apiUrl);
        }

        private async Task GetIncomeCategories(HttpClient httpClient)
        {
            var apiUrl = $"{_config["ApiUrl"]}/Finance/incomeCategories/{Id}";
            IncomeCategories = await GetApiResponse<Dictionary<string, decimal>>(httpClient, apiUrl);
        }

        private async Task<T> GetApiResponse<T>(HttpClient httpClient, string apiUrl)
        {
            var response = await httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                Unauthorized = true;
                return default;
            }
            else
            {
                response.EnsureSuccessStatusCode();
                return default;
            }
        }
    }
}
