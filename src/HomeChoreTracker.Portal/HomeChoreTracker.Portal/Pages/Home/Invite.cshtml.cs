using HomeChoreTracker.Portal.Models.Home;
using HomeChoreTracker.Portal.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Home
{
    public class InviteModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		[BindProperty]
		public int Id { get; set; }

		[BindProperty]
		public InviteUserRequest InviteUserRequest { get; set; }
        public bool Unauthorized { get; set; }

        public InviteModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

                var apiUrl = $"{_config["ApiUrl"]}/Home/CheckOrHomeMember/{id}";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return Page();
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Unauthorized = true;
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {response.ReasonPhrase}");
                    return Page();
                }
            }
        }


		public async Task<IActionResult> OnPostGenerateInvitationAsync()
		{
			if (!ModelState.IsValid)
			{
				return await OnGetAsync(Id);
			}

			try
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
					var apiUrl = _config["ApiUrl"] + "/Home/GenerateInvitation";
					InviteUserRequest.HomeId = Id;
					var response = await httpClient.PostAsJsonAsync(apiUrl, InviteUserRequest);

					if (response.IsSuccessStatusCode)
					{
						TempData["ToastType"] = "success";
						TempData["ToastMessage"] = "Invitation to home was sent successfully.";

						return Page();
					}
					else
					{
						var errorMessage = await response.Content.ReadAsStringAsync();
						ModelState.AddModelError(string.Empty, errorMessage);
						return Page();
					}
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				return await OnGetAsync(Id);
			}
		}
	}
}
