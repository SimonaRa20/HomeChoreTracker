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
using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Contracts.Gamification;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHomeRepository _homeRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IHomeChoreRepository _homeChoreRepository;
        private readonly IGamificationRepository _gamificationRepository;
        private readonly INotificationRepository _notificationRepository;

        public HomeController(IHomeRepository homeRepository, INotificationRepository notificationRepository, IAuthRepository authRepository, IHomeChoreRepository homeChoreRepository, IGamificationRepository gamificationRepository, IConfiguration config)
        {
            _homeRepository = homeRepository;
            _authRepository = authRepository;
            _homeChoreRepository = homeChoreRepository;
            _gamificationRepository = gamificationRepository;
            _notificationRepository = notificationRepository;
            _config = config;
        }

        [HttpPost]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> CreateHome([FromBody] HomeRequest homeRequest)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (await _homeRepository.CheckOrExistTitle(homeRequest))
            {
                return new ObjectResult("Home with the same title already exists.")
                {
                    StatusCode = (int)HttpStatusCode.UnprocessableEntity
                };
            }

            var home = await _homeRepository.CreateHome(homeRequest, userId);
            return Ok(home);
        }

        [HttpGet]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetAll()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            List<Home> homes = await _homeRepository.GetAll(userId);

            return Ok(homes);
        }

		[HttpGet("Level")]
		[Authorize(Roles = Role.User)]
		public async Task<IActionResult> GetMaxLevel()
		{
			GamificationLevel level = await _gamificationRepository.GetMaxLevel();

			return Ok(level.LevelId);
		}


		[HttpGet("Level/{homeId}")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetHomePlant(int homeId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            Home home = await _homeRepository.GetHome(homeId);

            if (home == null)
            {
                return NotFound("This home not found.");
            }

            bool isMember = await _homeRepository.OrHomeMember(homeId, userId);

            if(!isMember)
            {
                return Forbid();
            }

            List<PointsHistory> pointsHistory = await _gamificationRepository.GetPointsHistoryByHomeId(homeId);

            int sum = 0;

            foreach (var history in pointsHistory)
            {
                
                if(history.TaskId != null)
                {
                    TaskAssignment taskAssignment = await _homeChoreRepository.GetTaskAssigment((int)history.TaskId);
                    sum += (int)taskAssignment.Points;
                }
                else
                {
                    sum += (int)history.EarnedPoints;
                }
                
            }

            GamificationLevel gamificationLevel = await _gamificationRepository.GetGamificationLevel(home.GamificationLevelId);

            GamificationLevel gamificationNextLevel = await _gamificationRepository.GetGamificationLevel(home.GamificationLevelId + 1);

            LevelRequest levelRequest = new LevelRequest
            {
                EarnedPoints = sum,
                MaxPoints = (int)gamificationNextLevel.PointsRequired,
                GamificationLevelId = home.GamificationLevelId,
                GamificationLevel = gamificationLevel,
            };

            return Ok(levelRequest);
        }

        [HttpPut("Level/{homeId}")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> UpdateHomePlant(int homeId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            Home home = await _homeRepository.GetHome(homeId);

            if (home == null)
            {
                return NotFound("This home not found.");
            }

            bool isMember = await _homeRepository.OrHomeMember(homeId, userId);

            if (!isMember)
            {
                return Forbid();
            }

            GamificationLevel gamificationNowLevel = await _gamificationRepository.GetGamificationLevel(home.GamificationLevelId);
            GamificationLevel gamificationNextLevel = await _gamificationRepository.GetGamificationLevel(home.GamificationLevelId + 1);
            PointsHistory pointsHistory = new PointsHistory
            {
                EarnedPoints = -(int)gamificationNextLevel.PointsRequired,
                HomeId = homeId,
                Text = $"Upgrade home plant from {gamificationNowLevel.LevelId} level to {gamificationNextLevel.LevelId} level",
                EarnedDate = DateTime.Now
            };
            await _gamificationRepository.AddPointsHistory(pointsHistory);

            var members = await _homeRepository.GetHomeMembers(homeId);

            foreach (var member in members)
            {

                var getMember = await _authRepository.GetUserById((int)member.HomeMemberId);
                Notification notification = new Notification
                {
                    Title = $"Home plant was upgraded from {gamificationNowLevel.LevelId} level to {gamificationNextLevel.LevelId} level",
                    IsRead = false,
                    Time = DateTime.Now,
                    UserId = (int)member.HomeMemberId,
                    User = getMember,
                };

                await _notificationRepository.CreateNotification(notification);
            }

            home.GamificationLevel = gamificationNextLevel;
            home.GamificationLevelId = gamificationNextLevel.Id;
            await _homeRepository.Update(home);

            return Ok("Upgrade home level");
        }

        [HttpGet("{homeId}/skip{skip}/take{take}")]
        [Authorize]
        public async Task<IActionResult> GetPointsHistoryBase(int homeId, int skip = 0, int take = 10)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

                bool isMember = await _homeRepository.OrHomeMember(homeId, userId);

                if (!isMember)
                {
                    return Forbid();
                }

                var pointsHistory = await _gamificationRepository.GetPointsHistoryByHomeId(homeId);

                List<PointsHistory> points = pointsHistory.OrderBy(h => h.EarnedDate).Skip(skip).Take(take).ToList();
                List<PointsResponse> response = new List<PointsResponse>();

                foreach(var point in points)
                {
                    PointsResponse history = new PointsResponse
                    {
                        Text = point.Text,
                        Time = point.EarnedDate,
                        Points = point.EarnedPoints
                    };

                    response.Add(history);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching points history: {ex.Message}");
            }
        }

        [HttpGet("{homeId}")]
        [Authorize]
        public async Task<IActionResult> GetPointsHistoryBase(int homeId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                bool isMember = await _homeRepository.OrHomeMember(homeId, userId);

                if (!isMember)
                {
                    return Forbid();
                }


                var pointsHistory = await _gamificationRepository.GetPointsHistoryByHomeId(homeId);
                List<PointsHistory> points = pointsHistory.OrderBy(h => h.EarnedDate).ToList();
                List<PointsResponse> response = new List<PointsResponse>();

                foreach (var point in pointsHistory)
                {
                    PointsResponse history = new PointsResponse
                    {
                        Text = point.Text,
                        Time = point.EarnedDate,
                        Points = point.EarnedPoints
                    };

                    response.Add(history);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching points history: {ex.Message}");
            }
        }

        [HttpGet("CheckOrHomeMember/{homeId}")]
        [Authorize]
        public async Task<IActionResult> CheckOrHomeMember(int homeId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                bool isMember = await _homeRepository.OrHomeMember(homeId, userId);

                if (!isMember)
                {
                    return Forbid();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching points history: {ex.Message}");
            }
        }

        [Route("GenerateInvitation")]
        [HttpPost]
        public async Task<ActionResult> GenerateInvitation([FromBody] InviteUserRequest inviteUserRequest)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            var CheckOrIsMember = await _authRepository.GetUserByEmail(inviteUserRequest.InviteeEmail);

            if (CheckOrIsMember != null)
            {
                var checkOrHomeMember = await _homeRepository.OrHomeMember(inviteUserRequest.HomeId, CheckOrIsMember.Id);
                if (checkOrHomeMember)
                {
                    return new ObjectResult("User already added to home.")
                    {
                        StatusCode = (int)HttpStatusCode.UnprocessableEntity
                    };
                }
            }

            var user = await _authRepository.GetUserByEmail(inviteUserRequest.InviteeEmail);
            User sender = await _authRepository.GetUserById(userId);

            string invitationToken = await _homeRepository.InviteUserToHome(userId, inviteUserRequest.HomeId, inviteUserRequest.InviteeEmail);

            string invitationLink = $"{_config["AppUrl"]}/Homes/Invite?token={HttpUtility.UrlEncode(invitationToken)}";

            SendInvitationEmail(inviteUserRequest.InviteeEmail, invitationLink, sender);

            return Ok("Invitation sent successfully.");
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
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        [Route("InvitationAnswer")]
        [HttpPost]
        public async Task<ActionResult> InvitationAnswer([FromBody] InvitationAnswerRequest invitation)
        {
            var existingInvitation = await _homeRepository.GetInvitationByToken(invitation.Token);

            if (existingInvitation == null)
            {
                return NotFound("This invitation not found. Check your email link. Or ask your friend new invitation.");
            }

            if (existingInvitation.ExpirationDate < DateTime.UtcNow)
            {
                return BadRequest("This invitation expiration date finished. Ask your friend new invitation.");
            }

            var user = await _authRepository.GetUserByEmail(existingInvitation.InviteeEmail);

            if (user == null)
            {
                return BadRequest("First, create an account with the email address to which you received this invitation, and then try to confirm the invitation.");
            }

            if (invitation.IsAccept)
            {
                var userHome = new UserHomes
                {
                    UserId = user.Id,
                    HomeId = existingInvitation.HomeId,
                };

                await _homeRepository.AddToHome(userHome);
                await _homeRepository.RemoveInvitation(existingInvitation);

                var members = await _homeRepository.GetHomeMembers(existingInvitation.HomeId);

                foreach (var member in members)
                {
                    if (member.HomeMemberId != user.Id)
                    {
                        var getMember = await _authRepository.GetUserById((int)member.HomeMemberId);
                        Notification notification = new Notification
                        {
                            Title = $"User '{user.UserName}' join to home",
                            IsRead = false,
                            Time = DateTime.Now,
                            UserId = (int)member.HomeMemberId,
                            User = getMember,
                        };

                        await _notificationRepository.CreateNotification(notification);
                    }
                }

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
                return Forbid();
            }

            var members = await _homeRepository.GetHomeMembers(homeId);

            return Ok(members);
        }
    }
}
