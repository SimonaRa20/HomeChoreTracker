using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeChoreTracker.Portal.Pages.Homes
{
	public class IndexModel : PageModel
	{
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<HomeResponse> Homes { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
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
	}
}
