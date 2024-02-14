using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeChoreTracker.Portal.Pages.Homes
{
	public class IndexModel : PageModel
	{
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        public HomeRequest CreateHome { get; set; }

		[BindProperty]
		public InviteUserRequest InviteUserRequest { get; set; } 

        public List<HomeResponse> Homes { get; set; }

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

				var apiUrl = $"{_config["ApiUrl"]}/Home";

				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					Homes = await response.Content.ReadFromJsonAsync<List<HomeResponse>>();
					return Page();
				}
				else
				{
					ModelState.AddModelError(string.Empty, $"Failed to retrieve data: {response.ReasonPhrase}");
					return Page();
				}
			}
		}


		public async Task<IActionResult> OnPostCreateHomeAsync()
        {
			ClearFieldErrors(key => key.Contains("HomeId"));
			ClearFieldErrors(key => key.Contains("InviteeEmail"));
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
                    var apiUrl = _config["ApiUrl"] + "/Home";

                    var response = await httpClient.PostAsJsonAsync(apiUrl, CreateHome);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
                        await OnGetAsync();
                        return Page();
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

		public async Task<IActionResult> OnPostGenerateInvitationAsync()
		{
			ClearFieldErrors(key => key.Contains("Title"));
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
					var apiUrl = _config["ApiUrl"] + "/Home/GenerateInvitation";

					var response = await httpClient.PostAsJsonAsync(apiUrl, InviteUserRequest);

					if (response.IsSuccessStatusCode)
					{

						return RedirectToAction("Index");
					}
					else
					{
						await OnGetAsync();
						ModelState.AddModelError(string.Empty, $"Error: {response.StatusCode}");
						return Page();
					}
				}
			}
			catch (Exception ex)
			{
				await OnGetAsync();
				ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
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
