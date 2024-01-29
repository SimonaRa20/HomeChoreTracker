using HomeChoreTracker.Portal.Models.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeChoreTracker.Portal.Models;

namespace HomeChoreTracker.Portal.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Auth/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginRequest userLoginRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(userLoginRequest);
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var apiUrl = _config["ApiUrl"] + "/Auth/Login";
                var response = await httpClient.PostAsJsonAsync(apiUrl, userLoginRequest);

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

                    var authProperties = new AuthenticationProperties
                    {
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ErrorRequest>();
                    ViewData["ErrorMessage"] = errorResponse.ErrorMessage;
                    return View(userLoginRequest);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        //public IActionResult ForgotPassword()
        //{
        //    return View("~/Views/Auth/ForgotPassword.cshtml");
        //}

        public IActionResult Register()
        {
            return View("~/Views/Auth/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterRequestForm userRegisterRequest)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = string.Join("\n", ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .Where(msg => !string.IsNullOrEmpty(msg)));

                ViewData["ErrorMessage"] = errorMessage;

                return View(userRegisterRequest);
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                UserRegisterRequest userRequest = new UserRegisterRequest 
                { 
                    UserName = userRegisterRequest.UserName,
                    Email = userRegisterRequest.Email,
                    Password = userRegisterRequest.Password,
                };

                var apiUrl = _config["ApiUrl"] + "/Auth/Register";
                var response = await httpClient.PostAsJsonAsync(apiUrl, userRequest);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ErrorRequest>();
                    ViewData["ErrorMessage"] = errorResponse.ErrorMessage;
                    return View(userRegisterRequest);
                }
            }
        }
    }
}
