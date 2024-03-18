using HomeChoreTracker.Api.Constants;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Contracts.Auth;
using HomeChoreTracker.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public AuthController(IConfiguration config, IAuthRepository authRepository, IMapper mapper)
        {
            _config = config;
            _authRepository = authRepository;
            _mapper = mapper;
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterRequest user)
        {
            List<string> errors = new List<string>();

            if (user.Email.IsNullOrEmpty() || !user.Email.Contains('@'))
            {
                return new ObjectResult("Invalid email format.")
                {
                    StatusCode = (int)HttpStatusCode.UnprocessableEntity
                };
            }

            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 8)
            {
                return new ObjectResult("Password should be a minimum of 8 characters.")
                {
                    StatusCode = (int)HttpStatusCode.UnprocessableEntity
                };
            }
            var checkUser = await _authRepository.GetUserByEmail(user.Email);
            if (checkUser != null)
            {
                return new ObjectResult("User with the same email already exists.")
                {
                    StatusCode = (int)HttpStatusCode.UnprocessableEntity
                };
            }

            try
            {
                var hashedPassword = HashPassword(user.Password);
                user.Password = hashedPassword;

                User userDto = _mapper.Map<User>(user);
                userDto.Role = Role.User;
                userDto.Morning = true;
                userDto.MiddleDay = true;
                userDto.Evening = true;
                await _authRepository.AddUser(userDto);
                await _authRepository.Save();

                var userResponse = new UserRegisterResponse
                {
                    Id = userDto.Id,
                    UserName = userDto.UserName,
                    Email = userDto.Email
                };

                return Created("", userResponse);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while processing the request.");
            }
        }

        [Route("Login")]
        [HttpPost]
        public async Task<ActionResult> LoginAsync([FromBody] UserLoginRequest userLogin)
        {
            var user = await Authenticate(userLogin);
            if (user != null)
            {
                var accessToken = GenerateAccessToken(user);

                var userLoginResponse = LoginResponse(user, accessToken);
                return Created("", userLoginResponse);
            }

            return NotFound("Invalid email or password. Please try again.");

        }

        [Route("RestorePassword")]
        [HttpPost]
        public async Task<ActionResult> RestorePassword([FromBody] string email)
        {
            var user = await _authRepository.GetUserByEmail(email);

            if (user != null)
            {
                string resetToken = GeneratePasswordResetToken(user);

                string resetLink = $"{_config["AppUrl"]}/Auth/RestorePassword?token={HttpUtility.UrlEncode(resetToken)}";

                SendPasswordResetEmail(email, resetLink);

                return Ok("Password reset link sent successfully.");
            }

            return NotFound("Invalid email. Please try again." );
        }

        private void SendPasswordResetEmail(string email, string resetLink)
        {
            try
            {
                string fromMail = _config["EmailConfigServer:Email"];
                string fromPassword = _config["EmailConfigServer:Password"];

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = "Password Reset";
                message.To.Add(new MailAddress(email));
                message.Body = $"<html><body>Click the following link to reset your password: <a href=\"{resetLink}\">{resetLink}</a></body></html>";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient(_config["EmailConfigServer:Server"])
                {
                    Port = int.Parse(_config["EmailConfigServer:Port"]),
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                };

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        [Route("RestoreForgotPassword")]
        [HttpPut]
        public async Task<IActionResult> RestoreForgotPassword(UserRestorePasswordRequest restoreData)
        {
            var userId = ExtractUserIdFromToken(restoreData.Token);

            if (userId == 0)
            {
                return BadRequest("Invalid token.");
            }

            User user = await _authRepository.GetUserById(userId);

            if (user != null)
            {
                user.Password = HashPassword(restoreData.Password);

                _authRepository.Save();
                return Ok();
            }

            return BadRequest("User not found.");
        }

        private int ExtractUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null && jsonToken.Claims != null)
                {
                    var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "unique_name");

                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                    {
                        return userId;
                    }
                }
            }
            catch
            {
                // Token validation failed
            }
            return 0;
        }

        private string GeneratePasswordResetToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<User> Authenticate(UserLoginRequest userLogin)
        {
            var currentUser = await _authRepository.GetUserByEmail(userLogin.Email);

            if (currentUser != null)
            {
                if (VerifyPassword(userLogin.Password, currentUser.Password))
                {
                    return currentUser;
                }
            }

            return null;
        }
        private UserLoginResponse LoginResponse(User user, string accessToken)
        {
            UserLoginResponse response = new(user.Id, user.UserName, user.Role, accessToken);
            return response;
        }

        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            if (HashPassword(enteredPassword) == storedHashedPassword)
            {
                return true;
            }
            return false;
        }

        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            var saltValue = "dsdjiajeefiajofijaoifjoaijfoiajgorjag";
            byte[] salt = Encoding.ASCII.GetBytes(saltValue);
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashedPassword;
        }
    }
}
