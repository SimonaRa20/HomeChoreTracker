using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.ViewComponents
{
    public class PurchaseHomesListViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ClaimsPrincipal _user;

        public PurchaseHomesListViewComponent(IHttpClientFactory httpClientFactory, IConfiguration configuration, ClaimsPrincipal user)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
            _user = user;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var token = _user.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_config["ApiUrl"]}/Home";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var homes = await response.Content.ReadFromJsonAsync<List<HomeResponse>>();
                    return View(homes);
                }
                else
                {
                    return Content("Failed to retrieve homes list");
                }
            }
        }
    }
}
