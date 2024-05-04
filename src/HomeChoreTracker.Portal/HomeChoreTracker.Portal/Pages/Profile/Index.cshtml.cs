using HomeChoreTracker.Portal.Models.Profile;
using HomeChoreTracker.Portal.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        public UserProfileResponse UserProfile { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var token = User.FindFirstValue("Token");
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = _config["ApiUrl"] + "/User";
                    var response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        UserProfile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
                    }
                    return Page();
                }
            }
            return Redirect("/");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var token = User.FindFirstValue("Token");
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = _config["ApiUrl"] + "/User/Update";
                    var response = await httpClient.PutAsJsonAsync(apiUrl, UserProfile);

                    if (response.IsSuccessStatusCode)
                    {
						
						UserProfile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
                        TempData["ToastType"] = "success";
						TempData["ToastMessage"] = "Profile was updated successfully.";
						return Page();
					}
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, errorMessage);
						await OnGetAsync();
						return Page();
					}
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
