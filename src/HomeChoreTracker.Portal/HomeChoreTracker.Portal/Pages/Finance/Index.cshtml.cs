using HomeChoreTracker.Portal.Models.Finance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

		public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration config)
		{
			_httpClientFactory = httpClientFactory;
			_config = config;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var apiUrlBalance = $"{_config["ApiUrl"]}/Finance/totalBalance";
				var responseBalance = await httpClient.GetAsync(apiUrlBalance);

                var apiIncome = $"{_config["ApiUrl"]}/Finance/totalIncome";
                var responseIncome = await httpClient.GetAsync(apiIncome);

                var apiUrlExpense = $"{_config["ApiUrl"]}/Finance/totalExpense";
                var responseExpense= await httpClient.GetAsync(apiUrlExpense);

				var apiUrl = $"{_config["ApiUrl"]}/Finance/monthlySummary";
				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode && responseBalance.IsSuccessStatusCode && responseIncome.IsSuccessStatusCode && responseExpense.IsSuccessStatusCode)
				{
					MonthlySummaries = await response.Content.ReadFromJsonAsync<List<MonthlySummary>>();
					CurrentMonthTotalBalance = await responseBalance.Content.ReadFromJsonAsync<double>();
                    CurrentMonthTotalIncome = await responseIncome.Content.ReadFromJsonAsync<double>();
                    CurrentMonthTotalExpense = await responseExpense.Content.ReadFromJsonAsync<double>();

                    return Page();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {responseBalance.ReasonPhrase}");
					return Page();
				}
			}
		}
	}
}
