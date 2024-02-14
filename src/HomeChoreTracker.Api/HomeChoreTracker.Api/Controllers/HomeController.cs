using AutoMapper;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Web;
using Microsoft.EntityFrameworkCore;
using HomeChoreTracker.Api.Contracts.User;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHomeRepository _homeRepository;
        private readonly IAuthRepository _authRepository;

        public HomeController(IHomeRepository homeRepository, IAuthRepository authRepository, IConfiguration config)
        {
            _homeRepository = homeRepository;
            _authRepository = authRepository;
            _config = config;
        }

        [HttpPost]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> CreateHome([FromBody] HomeRequest homeRequest)
        {
            // Get the user ID from your authentication system
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            // Call the repository method to create the home
            await _homeRepository.CreateHome(homeRequest, userId);

            return Ok($"Home created successfully");
        }

        [HttpGet]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetAll()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            List<Home> homes = await _homeRepository.GetAll(userId);

            return Ok(homes);
        }

        [Route("GenerateInvitation")]
        [HttpPost]
        public async Task<ActionResult> GenerateInvitation([FromBody] InviteUserRequest inviteUserRequest)
        {
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

			var user = await _authRepository.GetUserByEmail(inviteUserRequest.InviteeEmail);
			User sender = await _authRepository.GetUserById(userId);
			if (user != null)
            {
                // Generate invitation token
                string invitationToken = await _homeRepository.InviteUserToHome(userId, inviteUserRequest.HomeId, inviteUserRequest.InviteeEmail);

                // Construct invitation link
                string invitationLink = $"{_config["AppUrl"]}/Home/Invite?token={HttpUtility.UrlEncode(invitationToken)}";
               

                // Send invitation email
                SendInvitationEmail(inviteUserRequest.InviteeEmail, invitationLink, sender);

                return Ok("Invitation sent successfully.");
            }

            return NotFound("User not found.");
        }

        private async void SendInvitationEmail(string email, string invitationLink, User user)
        {
            try
            {
                string registerLink = $"{_config["AppUrl"]}/Auth/Register";

                string fromMail = _config["EmailConfigServer:Email"];
                string fromPassword = _config["EmailConfigServer:Password"];

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = "Invitation to Home";
                message.To.Add(new MailAddress(email));
                message.Body = $@"<html><body>
                            <p>You have been invited to join a home.</p>
                            <p>You were invited by {user.Email}.</p>
                            <p>Click the following link to accept the invitation: <a href=""{invitationLink}"">{invitationLink}</a></p>
                            <p>If you don't have an account, please register <a href=""{registerLink}"">here</a> and then press on the invitation link.</p>
                         </body></html>";
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
                // Log the exception
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw; // Rethrow the exception to propagate it
            }
        }

        [Route("InvitationAnswer")]
        [HttpPost]
        public async Task<ActionResult> InvitationAnswer([FromBody]InvitationAnswerRequest invitation)
        {
            var existingInvitation = await _homeRepository.GetInvitationByToken(invitation.Token);
            var user = await _authRepository.GetUserByEmail(existingInvitation.InviteeEmail);
            if (existingInvitation == null)
            {
                return NotFound("Invitation not found.");
            }

            if (invitation.IsAccept)
            {
                var userHome = new UserHomes
                {
                    UserId = user.Id,
                    HomeId = existingInvitation.HomeId,
                    HomeRole = HomeRole.HomeUser,
                };

                await _homeRepository.AddToHome(userHome);
                await _homeRepository.RemoveInvitation(existingInvitation);

                return Ok("Invitation accepted successfully.");
            }
            else
            {
                await _homeRepository.RemoveInvitation(existingInvitation);
                return Ok("Invitation declined and removed.");
            }
        }
        [HttpGet("HomeMembers")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetHomeMembers(int homeId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            var isMember = await _homeRepository.OrHomeMember(homeId, userId);

            if (!isMember)
            {
                return Forbid(); // User is not a member of the home, forbid access
            }

            var members = await _homeRepository.GetHomeMembers(homeId);

            return Ok(members);
        }
    }
}
