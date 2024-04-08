using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using HomeChoreTracker.Portal.Models.Auth;

namespace HomeChoreTracker.Portal.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        [BindProperty]
        public UserLoginRequest UserLoginRequest { get; set; }

        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var apiUrl = _config["ApiUrl"] + "/Auth/Login";
                var response = await httpClient.PostAsJsonAsync(apiUrl, UserLoginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<UserLoginResponse>();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, loginResponse.Id.ToString()),
                        new Claim(ClaimTypes.Name, loginResponse.UserName),
                        new Claim(ClaimTypes.Role, loginResponse.Role),
                        new Claim("Token", loginResponse.Token)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties {};

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToPage("/Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return Page();
                }
            }
        }
    }
}
