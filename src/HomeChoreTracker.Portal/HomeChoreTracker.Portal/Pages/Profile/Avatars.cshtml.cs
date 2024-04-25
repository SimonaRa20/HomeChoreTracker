using HomeChoreTracker.Portal.Models.Avatar;
using HomeChoreTracker.Portal.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Profile
{
	public class AvatarsModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public int UserPoints { get; set; }
		public List<AvatarSelectionResponse> AvatarsResponses { get; set; }
		public AvatarResponse UserAvatar { get; set; }

		public AvatarsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
					var apiUrlInterval = _config["ApiUrl"] + "/Avatar/UserPoints";
					var response = await httpClient.GetAsync(apiUrlInterval);

					if (response.IsSuccessStatusCode)
					{
						UserPoints = await response.Content.ReadFromJsonAsync<int>();

						var apiUrl = _config["ApiUrl"] + "/Avatar/GetAvatars";
						var responseAvatars = await httpClient.GetAsync(apiUrl);

						if (responseAvatars.IsSuccessStatusCode)
						{
							AvatarsResponses = await responseAvatars.Content.ReadFromJsonAsync<List<AvatarSelectionResponse>>();
							var apiUrlUserAvatar = _config["ApiUrl"] + "/Avatar/GetUserAvatar";
							var responseUserAvatar = await httpClient.GetAsync(apiUrlUserAvatar);

							if (responseUserAvatar.IsSuccessStatusCode)
							{
								if (responseUserAvatar.StatusCode == System.Net.HttpStatusCode.NoContent)
								{
									UserAvatar = new AvatarResponse();
								}
								else
								{
									UserAvatar = await responseUserAvatar.Content.ReadFromJsonAsync<AvatarResponse>();
								}
							}
						}
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

		public async Task<IActionResult> OnPostBuyAsync(int avatarId)
		{
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var token = User.FindFirstValue("Token");
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = _config["ApiUrl"] + $"/Avatar/Buy/{avatarId}";

                    var response = await httpClient.PutAsync(apiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        return await OnGetAsync();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostSetAsync(int avatarId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var token = User.FindFirstValue("Token");
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = _config["ApiUrl"] + $"/Avatar/Set/{avatarId}";

                    var response = await httpClient.PutAsync(apiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        return await OnGetAsync();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}
