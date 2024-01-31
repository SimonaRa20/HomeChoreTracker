using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Controllers
{
    public class HomeChoreBaseController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public HomeChoreBaseController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var token = User.FindFirstValue("Token");

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = _config["ApiUrl"] + "/HomeChoreBase";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var homeChoreBases = await response.Content.ReadFromJsonAsync<List<HomeChoreBaseResponse>>();
                    return View(homeChoreBases);
                }
                else
                {
                    // Handle error
                    return View("Error");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(HomeChoreBaseRequest homeChoreBaseRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(homeChoreBaseRequest);
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var apiUrl = _config["ApiUrl"] + "/HomeChoreBase/CreateHomeChoreBase";
                var response = await httpClient.PostAsJsonAsync(apiUrl, homeChoreBaseRequest);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle error
                    return View("Error");
                }
            }
        }
    }
}
