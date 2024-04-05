using HomeChoreTracker.Api.Contracts.Notification;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;

        public NotificationController(INotificationRepository notificationRepository, IUserRepository userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotReadNotifications()
        {
            try
            {
                int id = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

                var response = await _notificationRepository.GetNotReadNotifications(id);
                List<Notification> result = response.OrderByDescending(notification => notification.Time).ToList();
                List<NotificationResponse> notifications = new List<NotificationResponse>();
                foreach (var notification in result)
                {
                    NotificationResponse notificationResponse = new NotificationResponse
                    {
                        Title = notification.Title,
                        IsRead = notification.IsRead,
                        Time = notification.Time,
                    };
                    notifications.Add(notificationResponse);
                }
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                int id = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

                var response = await _notificationRepository.GetNotifications(id);
                List<Notification> result = response.OrderByDescending(notification => notification.Time).ToList();
                List<NotificationResponse> notifications = new List<NotificationResponse>();
                foreach (var notification in result)
                {
                    NotificationResponse notificationResponse = new NotificationResponse
                    {
                        Title = notification.Title,
                        IsRead = notification.IsRead,
                        Time = notification.Time,
                    };
                    notifications.Add(notificationResponse);
                }

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Authorize]
        public async Task<IActionResult> SetRead()
        {
            try
            {
                int id = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                List<Notification> response = await _notificationRepository.GetNotifications(id);

                if(response == null)
                {
                    return Ok("Not found notifications");
                }

                foreach(var notification in response)
                {
                    if(!notification.IsRead)
                    {
                        notification.IsRead = true;
                        await _notificationRepository.Update(notification);
                    }
                }

                return Ok("Marked as read all notifications");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
