using HomeChoreTracker.Portal.Models.Finance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using HomeChoreTracker.Portal.Constants;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Finance
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public double CurrentMonthTotalBalance { get; set; }
        public double CurrentMonthTotalIncome { get; set; }
        public double CurrentMonthTotalExpense { get; set; }
        public List<MonthlySummary> MonthlySummaries { get; set; }
        public Dictionary<string, int> ExpenseCategories { get; set; }
        public Dictionary<string, int> IncomeCategories { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<IActionResult> OnGetAsync()
        {
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
            var apiUrlBalance = $"{_config["ApiUrl"]}/Finance/totalBalance";
            var apiUrlIncome = $"{_config["ApiUrl"]}/Finance/totalIncome";
            var apiUrlExpense = $"{_config["ApiUrl"]}/Finance/totalExpense";

            CurrentMonthTotalBalance = await GetApiResponse<double>(httpClient, apiUrlBalance);
            CurrentMonthTotalIncome = await GetApiResponse<double>(httpClient, apiUrlIncome);
            CurrentMonthTotalExpense = await GetApiResponse<double>(httpClient, apiUrlExpense);
        }

        private async Task GetMonthlySummaries(HttpClient httpClient)
        {
            var apiUrl = $"{_config["ApiUrl"]}/Finance/monthlySummary";
            MonthlySummaries = await GetApiResponse<List<MonthlySummary>>(httpClient, apiUrl);
        }

        private async Task GetExpenseCategories(HttpClient httpClient)
        {
            var apiUrl = $"{_config["ApiUrl"]}/Finance/expenseCategories";
            ExpenseCategories = await GetApiResponse<Dictionary<string, int>>(httpClient, apiUrl);
        }

        private async Task GetIncomeCategories(HttpClient httpClient)
        {
            var apiUrl = $"{_config["ApiUrl"]}/Finance/incomeCategories";
            IncomeCategories = await GetApiResponse<Dictionary<string, int>>(httpClient, apiUrl);
        }

        private async Task<T> GetApiResponse<T>(HttpClient httpClient, string apiUrl)
        {
            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode(); // Throw if not a success code

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
