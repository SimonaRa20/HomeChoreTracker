using HomeChoreTracker.Portal.Constants;
using HomeChoreTracker.Portal.Models.Finance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public List<MonthlySummary> MonthlySummaries { get; set; }
        public Dictionary<FinancialCategory, int> ExpenseCategories { get; set; }
        public Dictionary<FinancialCategory, int> IncomeCategories { get; set; }


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

            CurrentMonthTotalBalance = await GetApiResponse<double>(httpClient, apiUrlBalance);
            CurrentMonthTotalIncome = await GetApiResponse<double>(httpClient, apiUrlIncome);
            CurrentMonthTotalExpense = await GetApiResponse<double>(httpClient, apiUrlExpense);
        }

        private async Task GetMonthlySummaries(HttpClient httpClient)
        {
            var apiUrl = $"{_config["ApiUrl"]}/Finance/monthlySummary/{Id}";
            MonthlySummaries = await GetApiResponse<List<MonthlySummary>>(httpClient, apiUrl);
        }

        private async Task GetExpenseCategories(HttpClient httpClient)
        {
            var apiUrl = $"{_config["ApiUrl"]}/Finance/expenseCategories/{Id}";
            ExpenseCategories = await GetApiResponse<Dictionary<FinancialCategory, int>>(httpClient, apiUrl);
        }

        private async Task GetIncomeCategories(HttpClient httpClient)
        {
            var apiUrl = $"{_config["ApiUrl"]}/Finance/incomeCategories/{Id}";
            IncomeCategories = await GetApiResponse<Dictionary<FinancialCategory, int>>(httpClient, apiUrl);
        }

        private async Task<T> GetApiResponse<T>(HttpClient httpClient, string apiUrl)
        {
            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode(); 

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
