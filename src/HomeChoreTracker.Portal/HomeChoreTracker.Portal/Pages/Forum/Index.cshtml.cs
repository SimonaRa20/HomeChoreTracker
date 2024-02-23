using HomeChoreTracker.Portal.Models.Forum;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Forum
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<AdviceResponse> Advices { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        public async Task<IActionResult> OnGetAsync(string search, string type)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/Forum";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Advices = await response.Content.ReadFromJsonAsync<List<AdviceResponse>>();

                    // Filter advices based on the search query
                    if (!string.IsNullOrEmpty(search))
                    {
                        Advices = Advices.Where(a => a.Title.Contains(search) || a.Description.Contains(search)).ToList();
                    }

                    // Filter advices based on the type
                    if (!string.IsNullOrEmpty(type))
                    {
                        Advices = Advices.Where(a => string.Equals(a.Type.ToString(), type, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    return Page();
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }


    }
}
