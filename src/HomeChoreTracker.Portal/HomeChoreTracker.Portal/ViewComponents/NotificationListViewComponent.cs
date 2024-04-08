using HomeChoreTracker.Portal.Models.Notification;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.ViewComponents
{
	public class NotificationListViewComponent : ViewComponent
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;
		private readonly ClaimsPrincipal _user;

		public NotificationListViewComponent(IHttpClientFactory httpClientFactory, IConfiguration configuration, ClaimsPrincipal user)
		{
			_httpClientFactory = httpClientFactory;
			_config = configuration;
			_user = user;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var token = _user.FindFirstValue("Token");
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				var apiUrl = $"{_config["ApiUrl"]}/Notification";
				var response = await httpClient.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					var notifications = await response.Content.ReadFromJsonAsync<List<NotificationResponse>>();
					return View(notifications);
				}
				else
				{
					return Content("Failed to retrieve homes list");
				}
			}
		}
	}
}
