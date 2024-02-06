using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("notifications")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetNotifications()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            var notifications = await _userRepository.GetUserNotifications(userId);

            return Ok(notifications);
        }

        [HttpPost("notifications/mark-as-read/{notificationId}")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            await _userRepository.MarkNotificationAsRead(notificationId);

            return Ok();
        }

    }
}
