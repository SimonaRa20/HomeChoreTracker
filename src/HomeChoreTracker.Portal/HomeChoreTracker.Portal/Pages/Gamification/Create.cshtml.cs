using HomeChoreTracker.Portal.Models.Gamification;
using HomeChoreTracker.Portal.Models.HomeChore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Gamification
{
	public class CreateModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public List<GamificationLevelResponse> LevelResponse { get; set; }
		public int Level { get; set; }

		[BindProperty]
		public GamificationLevelRequest CreateLevel { get; set; }

		public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var token = User.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var apiUrl = $"{_config["ApiUrl"]}/Gamification";

				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					LevelResponse = await response.Content.ReadFromJsonAsync<List<GamificationLevelResponse>>();
					Level = LevelResponse.Count() + 1;
					return Page();
				}
				else
				{
					return Page();
				}
			}
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
						var apiUrl = _config["ApiUrl"] + "/Gamification";

						using (var content = new MultipartFormDataContent())
						{
							if (CreateLevel.LevelId.HasValue)
							{
								content.Add(new StringContent(CreateLevel.LevelId.Value.ToString()), "LevelId");
							}

							if (CreateLevel.PointsRequired.HasValue)
							{
								content.Add(new StringContent(CreateLevel.PointsRequired.Value.ToString()), "PointsRequired");
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
								TempData["ToastMessage"] = "Level created successfully.";

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
