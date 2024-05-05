using HomeChoreTracker.Portal.Constants;
using HomeChoreTracker.Portal.Models.Avatar;
using HomeChoreTracker.Portal.Models.Gamification;
using HomeChoreTracker.Portal.Models.HomeChore;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
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

		[BindProperty(SupportsGet = true)]
		public int CurrentPage { get; set; } = 1;

		public int Count { get; set; }
		public int PageSize { get; set; } = 4;
		public int TotalPages { get; set; }
		public bool ShowPrevious => CurrentPage > 1;
		public bool ShowNext => CurrentPage < TotalPages;

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
				int pageSize = PageSize;
				int skip = (CurrentPage - 1) * pageSize;

				if (response.IsSuccessStatusCode)
                {
					List<AvatarResponse> list = await response.Content.ReadFromJsonAsync<List<AvatarResponse>>();
					Count = list.Count;
					TotalPages = (int)Math.Ceiling((double)Count / pageSize);
					AvatarsResponse = list.Skip(skip).Take(pageSize).ToList();
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
