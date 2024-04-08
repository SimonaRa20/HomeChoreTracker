using HomeChoreTracker.Portal.Models.Home;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using HomeChoreTracker.Portal.Models.Purchase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Portal.Pages.Purchase
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<Models.Purchase.Purchase> Purchases { get; set; }

        [BindProperty]
        public int PurchaseId { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            PurchaseId = id;
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/Purchase/{PurchaseId}";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Purchases = await response.Content.ReadFromJsonAsync<List<Models.Purchase.Purchase>>();
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {response.ReasonPhrase}");
                    return Page();
                }
            }
        }

		public async Task<IActionResult> OnGetPurchaseDetailAsync(int purchaseId)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/Purchase/purchase/{purchaseId}";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var purchase = await response.Content.ReadFromJsonAsync<Models.Purchase.Purchase>();
                    return new JsonResult(purchase);
                }
                else
                {
                    return BadRequest($"Failed to retrieve purchase data: {response.ReasonPhrase}");
                }
            }
        }

		public async Task<IActionResult> OnPostUpdateShoppingItemsAsync(int purchaseId)
		{
			var token = User.FindFirstValue("Token");
			var itemsToUpdateJson = HttpContext.Request.Form["itemsToUpdate"];

			if (!string.IsNullOrEmpty(itemsToUpdateJson))
			{
				var itemsToUpdate = JsonConvert.DeserializeObject<List<ShoppingItemUpdateRequest>>(itemsToUpdateJson);

				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

					var apiUrl = $"{_config["ApiUrl"]}/Purchase/UpdateShoppingItems";

					var response = await httpClient.PostAsJsonAsync(apiUrl, itemsToUpdate);

					if (response.IsSuccessStatusCode)
					{
						return RedirectToPage("/Purchase/Index", new { id = purchaseId });
					}
					else
					{
						return BadRequest($"Failed to update shopping items: {response.ReasonPhrase}");
					}
				}
			}
			else
			{
				return BadRequest("No data received from the form.");
			}
		}


		public async Task<IActionResult> OnPostUpdateShoppingPurchaseAsync()
		{
			var token = User.FindFirstValue("Token");
			var itemsToUpdateJson = HttpContext.Request.Form["itemsToUpdate"];

			if (!string.IsNullOrEmpty(itemsToUpdateJson))
			{
				var itemsToUpdate = JsonConvert.DeserializeObject<List<ShoppingPurchaseUpdate>>(itemsToUpdateJson);

				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

					int purchaseId = int.Parse(HttpContext.Request.Form["purchaseId"]);

					var apiUrl = $"{_config["ApiUrl"]}/Purchase/UpdatePurchase/{purchaseId}";

					var response = await httpClient.PostAsJsonAsync(apiUrl, itemsToUpdate);

					if (response.IsSuccessStatusCode)
					{
						return RedirectToPage("/Purchase/Index", new { id = PurchaseId });
					}
					else
					{
						return BadRequest($"Failed to update shopping items: {response.ReasonPhrase}");
					}
				}
			}
			else
			{
				return BadRequest("No data received from the form.");
			}
		}

		public async Task<IActionResult> OnPostDeleteAsync(int purchaseId, int selectedHomeId)
		{
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				var apiUrl = $"{_config["ApiUrl"]}/Purchase/{purchaseId}";

				var response = await httpClient.DeleteAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					return RedirectToPage("/Purchase/Index", new { id = selectedHomeId });
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to delete chore: {response.StatusCode}");
					return RedirectToPage("/Purchase/Index", new { id = selectedHomeId });
				}
			}
		}
	}
}
