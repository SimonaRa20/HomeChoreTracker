using HomeChoreTracker.Portal.Constants;
using HomeChoreTracker.Portal.Models.Avatar;
using HomeChoreTracker.Portal.Models.Gamification;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Avatars
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<AvatarResponse> AvatarsResponse { get; set; }
		[BindProperty]
		public AvatarUpdateRequest EditAvatar { get; set; }

		public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
			EditAvatar = new AvatarUpdateRequest();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/Avatar";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    AvatarsResponse = await response.Content.ReadFromJsonAsync<List<AvatarResponse>>();
                    return Page();
                }
                else
                {
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
				var apiUrl = $"{_config["ApiUrl"]}/Avatar/{id}";
				var response = await httpClient.GetAsync(apiUrl);

				var avatarDetailed = new AvatarResponse();
				if (response.IsSuccessStatusCode)
				{
					avatarDetailed = await response.Content.ReadFromJsonAsync<AvatarResponse>();

					if (avatarDetailed != null)
					{
						string avatarType = ((AvatarType)avatarDetailed.AvatarType).ToString();
						byte[] avatarImage = avatarDetailed.Image;

						return new JsonResult(new
						{
							id = avatarDetailed.Id,
							type = avatarType,
							image = avatarImage
						});
					}
					else
					{
						return NotFound("Avatar details not found.");
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
			if (!ModelState.IsValid)
			{
				await OnGetAsync();
				return Page();
			}

			try
			{
				var token = User.FindFirstValue("Token");
				using (var httpClient = _httpClientFactory.CreateClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
					var apiUrl = $"{_config["ApiUrl"]}/Avatar/{id}";

					using (var content = new MultipartFormDataContent())
					{
						EditAvatar.AvatarType = Enum.Parse<AvatarType>(Request.Form["editAvatarType"]);
						if (EditAvatar.AvatarType != null)
						{
							content.Add(new StringContent(EditAvatar.AvatarType.ToString()), "AvatarType");
						}
						var imageFile = Request.Form.Files["editNewImage"];
						if (imageFile != null)
						{
							using (var stream = new MemoryStream())
							{
								await imageFile.CopyToAsync(stream);
								content.Add(new ByteArrayContent(stream.ToArray()), "Image", imageFile.FileName);
							}
						}

						var response = await httpClient.PutAsync(apiUrl, content);

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
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
				await OnGetAsync();
				return Page();
			}
		}

	}
}
