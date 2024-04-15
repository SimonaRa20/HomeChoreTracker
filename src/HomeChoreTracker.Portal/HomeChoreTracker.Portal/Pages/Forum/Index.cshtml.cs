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
using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Pages.Forum
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<AdviceResponse> Advices { get; set; }
        public int UserId { get; set; }

        [BindProperty]
        public AdviceRequest EditAdvice { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string TypeFilter { get; set; }


        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
            EditAdvice = new AdviceRequest();
        }

        public async Task<IActionResult> OnGetAsync(string search)
        {
            var token = User.FindFirstValue("Token");
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_config["ApiUrl"]}/Forum";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Advices = await response.Content.ReadFromJsonAsync<List<AdviceResponse>>();

                    if (!string.IsNullOrEmpty(search))
                    {
                        Advices = Advices.Where(a => a.Title.Contains(search) || a.Description.Contains(search)).ToList();
                    }

                    return Page();
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_config["ApiUrl"]}/Forum/{id}";
                var response = await httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to delete chore: {response.StatusCode}");
                    return Page();
                }
            }
        }

        public async Task<IActionResult> OnGetDetailAsync(int id)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_config["ApiUrl"]}/Forum/{id}";
                var response = await httpClient.GetAsync(apiUrl);
                var adviceDetails = new AdviceDetailedResponse();

                if (response.IsSuccessStatusCode)
                {
                    adviceDetails = await response.Content.ReadFromJsonAsync<AdviceDetailedResponse>();

                    if (adviceDetails != null)
                    {
                        string adviceTypeText = ((HomeChoreType)adviceDetails.Type).ToString();
                        return new JsonResult(new
                        {
                            title = adviceDetails.Title,
                            adviceType = adviceTypeText,
                            description = adviceDetails.Description,
                            isPublic = adviceDetails.IsPublic,
                        });
                    }
                    else
                    {
                        return NotFound("Advice details not found.");
                    }
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            ClearFieldErrors(key => key == "Title");
            ClearFieldErrors(key => key == "Search");
            ClearFieldErrors(key => key == "TypeFilter");
            ClearFieldErrors(key => key == "Description");
            if (!ModelState.IsValid)
            {
                await OnGetAsync(null);
                return Page();
            }

            try
            {
                var token = User.FindFirstValue("Token");
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = $"{_config["ApiUrl"]}/Forum/{id}";
                    EditAdvice.Title = Request.Form["EditName"];
                    EditAdvice.Type = Enum.Parse<HomeChoreType>(Request.Form["EditAdviceType"]);
                    EditAdvice.Description = Request.Form["EditDescription"];
                    EditAdvice.IsPublic = bool.Parse(Request.Form["EditIsPublic"]);
                    var response = await httpClient.PutAsJsonAsync(apiUrl, EditAdvice);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToPage();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Failed to update chore: {response.StatusCode}");
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                await OnGetAsync(null);
                return Page();
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
