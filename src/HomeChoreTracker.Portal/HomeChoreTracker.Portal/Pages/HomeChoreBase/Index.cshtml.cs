using HomeChoreTracker.Portal.Constants;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace HomeChoreTracker.Portal.Pages.HomeChoreBase
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<HomeChoreBaseResponse> HomeChoreBases { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }

        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

        [BindProperty]
        public HomeChoreBaseEditRequest EditHomeChore { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
            EditHomeChore = new HomeChoreBaseEditRequest();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                int pageSize = PageSize;
                int skip = (CurrentPage - 1) * pageSize;

                var apiUrl = $"{_config["ApiUrl"]}/HomeChoreBase/skip{skip}/take{pageSize}";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    HomeChoreBases = await response.Content.ReadFromJsonAsync<List<HomeChoreBaseResponse>>();
                    var apiUrlAll = _config["ApiUrl"] + "/HomeChoreBase";
                    var totalCountResponse = await httpClient.GetAsync(apiUrlAll);
                    if (totalCountResponse.IsSuccessStatusCode)
                    {
                        List<HomeChoreBaseResponse> list = await totalCountResponse.Content.ReadFromJsonAsync<List<HomeChoreBaseResponse>>();
                        Count = list.Count;
                        TotalPages = (int)Math.Ceiling((double)Count / pageSize);
                    }

                    return Page();
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }

        public async Task<IActionResult> OnGetDetailAsync(int id)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/HomeChoreBase/{id}";
                var response = await httpClient.GetAsync(apiUrl);

                var choreDetails = new HomeChoreBaseResponse();
                if (response.IsSuccessStatusCode)
                {
                    choreDetails = await response.Content.ReadFromJsonAsync<HomeChoreBaseResponse>();

                    if (choreDetails != null)
                    {
                        string choreTypeText = ((HomeChoreType)choreDetails.ChoreType).ToString();
                        string frequencyText = ((Frequency)choreDetails.Frequency).ToString();
                        string descriptionText = choreDetails.Description ?? "-";
                        return new JsonResult(new
                        {
                            name = choreDetails.Name,
                            choreType = choreTypeText,
                            frequency = frequencyText,
                            description = descriptionText
                        });
                    }
                    else
                    {
                        return NotFound("Chore details not found.");
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
            ClearFieldErrors(key => key == "Name");

            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // Refresh the data
                return Page();
            }

            try
            {
                var token = User.FindFirstValue("Token");
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = $"{_config["ApiUrl"]}/HomeChoreBase/{id}";

                    // Update EditHomeChore with form values
                    EditHomeChore.Name = Request.Form["EditName"];
                    EditHomeChore.ChoreType = (int)Enum.Parse<HomeChoreType>(Request.Form["EditChoreType"]);
                    EditHomeChore.Frequency = (int)Enum.Parse<Frequency>(Request.Form["EditFrequency"]);
                    EditHomeChore.Description = Request.Form["EditDescription"];

                    var response = await httpClient.PutAsJsonAsync(apiUrl, EditHomeChore);

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
                await OnGetAsync(); // Refresh the data
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_config["ApiUrl"]}/HomeChoreBase/{id}";

                var response = await httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    // Handle deletion failure
                    ModelState.AddModelError(string.Empty, $"Failed to delete chore: {response.StatusCode}");
                    return Page();
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