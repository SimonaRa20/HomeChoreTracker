using HomeChoreTracker.Portal.Models.Purchase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using HomeChoreTracker.Portal.Models.HomeChore;

namespace HomeChoreTracker.Portal.Pages.Purchase
{
    public class AddPurchaseModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        public PurchaseRequest PurchaseRequest { get; set; }

		public List<HomeChoreResponse> HomeChoreResponse { get; set; }

		[BindProperty]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
		{
            Id = id;
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var apiHomeChoreUrl = $"{_config["ApiUrl"]}/HomeChore/{id}";
				var responseHomeChore = await httpClient.GetAsync(apiHomeChoreUrl);

				if (responseHomeChore.IsSuccessStatusCode)
				{
					HomeChoreResponse = await responseHomeChore.Content.ReadFromJsonAsync<List<HomeChoreResponse>>();
					return Page();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {responseHomeChore.ReasonPhrase}");
					return Page();
				}
			}
		}

        public AddPurchaseModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
            PurchaseRequest = new PurchaseRequest();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var token = User.FindFirstValue("Token");
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    PurchaseRequest.HomeId = Id;
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = _config["ApiUrl"] + "/Purchase";
                    var response = await httpClient.PostAsJsonAsync(apiUrl, PurchaseRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToPage("/Purchase/Index", new { id = Id });
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
    }
}
