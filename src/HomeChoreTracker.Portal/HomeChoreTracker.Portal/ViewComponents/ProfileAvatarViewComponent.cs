using HomeChoreTracker.Portal.Models.Avatar;
using HomeChoreTracker.Portal.Models.Home;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeChoreTracker.Portal.ViewComponents
{
    public class ProfileAvatarViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ClaimsPrincipal _user;

        public ProfileAvatarViewComponent(IHttpClientFactory httpClientFactory, IConfiguration configuration, ClaimsPrincipal user)
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
                var apiUrl = $"{_config["ApiUrl"]}/Avatar/GetUserAvatar";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var avatar = new AvatarResponse();
                    if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                    { 
                        avatar = await response.Content.ReadFromJsonAsync<AvatarResponse>();
                    }
                    
                    return View(avatar);
                }
                else
                {
                    return Content("Failed to retrieve avatar");
                }
            }
        }
    }
}
