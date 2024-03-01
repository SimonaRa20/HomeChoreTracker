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
        public int Id { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/Purchase/{Id}";

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

        public async Task<IActionResult> OnPostUpdateShoppingItemsAsync()
        {
            var token = User.FindFirstValue("Token");
            var itemsToUpdateJson = HttpContext.Request.Form["itemsToUpdate"];

            if (!string.IsNullOrEmpty(itemsToUpdateJson))
            {
                // Deserialize the JSON string to a list of ShoppingItemUpdateRequest
                var itemsToUpdate = JsonConvert.DeserializeObject<List<ShoppingItemUpdateRequest>>(itemsToUpdateJson);

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var apiUrl = $"{_config["ApiUrl"]}/Purchase/UpdateShoppingItems";

                    var response = await httpClient.PostAsJsonAsync(apiUrl, itemsToUpdate);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToPage("/Purchase/Index", new { id = Id });
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


    }
}
