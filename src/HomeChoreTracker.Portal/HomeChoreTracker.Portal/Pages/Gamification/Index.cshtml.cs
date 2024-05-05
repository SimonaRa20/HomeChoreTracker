using HomeChoreTracker.Portal.Models.Gamification;
using HomeChoreTracker.Portal.Models.HomeChore;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.Pages.Gamification
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<GamificationLevelResponse> LevelResponse { get; set; }

        [BindProperty]
        public GamificationLevelUpdateRequest EditLevel { get; set; }

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
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/Gamification";

                var response = await httpClient.GetAsync(apiUrl);
				int pageSize = PageSize;
				int skip = (CurrentPage - 1) * pageSize;

				if (response.IsSuccessStatusCode)
                {
					List<GamificationLevelResponse> list = await response.Content.ReadFromJsonAsync<List<GamificationLevelResponse>>();
					Count = list.Count;
					TotalPages = (int)Math.Ceiling((double)Count / pageSize);
					LevelResponse = list.Skip(skip).Take(pageSize).ToList();
					return Page();
                }
                else
                {
                    return Page();
                }
            }
        }

        public async Task<IActionResult> OnPostEditLevelAsync(int id)
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
                    var apiUrl = _config["ApiUrl"] + $"/Gamification/{id}";

                    using (var content = new MultipartFormDataContent())
                    {
                        var points = int.Parse(Request.Form["EditPointsRequired"]);
                        if (points != null)
                        {
                            content.Add(new StringContent(points.ToString()), "PointsRequired");
                        }

                        var imageFile = Request.Form.Files["EditImage"];
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
                            return await OnGetAsync();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
                            return Page();
                        }
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
