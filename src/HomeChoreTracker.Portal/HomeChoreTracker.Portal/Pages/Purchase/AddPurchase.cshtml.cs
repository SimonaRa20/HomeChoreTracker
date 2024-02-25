using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using HomeChoreTracker.Portal.Models.Purchase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace HomeChoreTracker.Portal.Pages.Purchase
{
    public class AddPurchaseModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        public PurchaseRequest Purchase { get; set; }

        public List<ProductResponse> Products { get; set; }

        [BindProperty]
        public int Id { get; set; }

        public AddPurchaseModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
            Purchase = new PurchaseRequest();
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            await LoadProducts();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ClearFieldErrors(key => key == "Items");
            if (!ModelState.IsValid)
            {
                await LoadProducts();
                return Page();
            }

            try
            {
                var token = User.FindFirstValue("Token");
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = _config["ApiUrl"] + "/Inventory/purchase";
                    Purchase.HomeId = Id;
                    var response = await httpClient.PostAsJsonAsync(apiUrl, Purchase);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToPage("/Purchase/Index", new { id = Purchase.HomeId });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
                        await LoadProducts();
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                await LoadProducts();
                return Page();
            }
        }

        private async Task LoadProducts()
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = _config["ApiUrl"] + $"/Inventory/products/{Id}";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to retrieve products: {response.ReasonPhrase}");
                }
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
