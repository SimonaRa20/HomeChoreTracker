using HomeChoreTracker.Portal.Models.Avatar;
using HomeChoreTracker.Portal.Models.Gamification;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace HomeChoreTracker.Portal.Pages.Avatars
{
    public class CreateModel : PageModel
    {
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		[BindProperty]
		public AvatarRequest CreateAvatar { get; set; }

		public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			try
			{
				if (User.Identity.IsAuthenticated)
				{
					var token = User.FindFirstValue("Token");
					using (var httpClient = _httpClientFactory.CreateClient())
					{
						httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
						var apiUrl = _config["ApiUrl"] + "/Avatar";

						using (var content = new MultipartFormDataContent())
						{
							if (CreateAvatar.AvatarType != null)
							{
								content.Add(new StringContent(CreateAvatar.AvatarType.ToString()), "AvatarType");
							}

							var imageFile = Request.Form.Files["Image"];
							if (imageFile != null)
							{
								using (var stream = new MemoryStream())
								{
									await imageFile.CopyToAsync(stream);
									content.Add(new ByteArrayContent(stream.ToArray()), "Image", imageFile.FileName);
								}
							}

							var response = await httpClient.PostAsync(apiUrl, content);

							if (response.IsSuccessStatusCode)
							{
                                TempData["ToastType"] = "success";
                                TempData["ToastMessage"] = "Avatar created successfully.";

                                return Page();
							}
							else
							{
								ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
								return Page();
							}
						}
					}
				}
				else
				{
					return RedirectToPage("/");
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
