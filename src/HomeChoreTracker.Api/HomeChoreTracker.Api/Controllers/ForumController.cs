using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Contracts.Forum;
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
	public class ForumController : Controller
    {
		private readonly IForumRepository _forumRepository;
		private readonly IUserRepository _userRepository;
		private readonly IGamificationRepository _gamificationRepository;
		private readonly INotificationRepository _notificationRepository;

        public ForumController(IForumRepository forumRepository, IUserRepository userRepository, IGamificationRepository gamificationRepository, INotificationRepository notificationRepository)
        {
            _forumRepository = forumRepository;
			_userRepository = userRepository;
			_gamificationRepository = gamificationRepository;
			_notificationRepository = notificationRepository;
        }

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> AddAdvice(AdviceRequest adviceRequest)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			var user = await _userRepository.GetUserById(userId);
			var advice = new Advice
			{
				Title = adviceRequest.Title,
				Time = DateTime.Now,
				Type = adviceRequest.Type,
				Description = adviceRequest.Description,
				IsPublic = adviceRequest.IsPublic,
				UserId = userId,
			};

			await _forumRepository.AddAdvice(advice);

            var hasBadge = await _gamificationRepository.UserHasCreateFirstAdviceBadge(userId);
            if (!hasBadge)
            {
                BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(userId);
                wallet.CreateFirstAdvice = true;
                await _gamificationRepository.UpdateBadgeWallet(wallet);

                Notification notification = new Notification
                {
                    Title = $"You earned badge 'Create first advice'",
                    IsRead = false,
                    Time = DateTime.Now,
                    UserId = (int)userId,
                    User = user,
                };

                await _notificationRepository.CreateNotification(notification);
            }
            return Ok("Advice added successfully");
		}

		[HttpDelete("{id}")]
		[Authorize]
		public async Task<IActionResult> DeleteAdviceById(int id)
		{
			await _forumRepository.Delete(id);
			return Ok($"Advice with ID {id} deleted successfully");
		}

		[HttpGet("{id}")]
		[Authorize]
		public async Task<IActionResult> GetAdvice(int id)
		{
			Advice advice = await _forumRepository.GetAdviceById(id);
			return Ok(advice);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetAdvices()
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			List<AdviceResponse> advices = await _forumRepository.GetAll(userId);
			return Ok(advices);
		}

		[HttpPut("{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateAdvice(int id, AdviceRequest adviceRequest)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			var adviceToUpdate = await _forumRepository.GetAdviceById(id);

			adviceToUpdate.Title = adviceRequest.Title;
			adviceToUpdate.Type = adviceRequest.Type;
			adviceToUpdate.Description = adviceRequest.Description;
			adviceToUpdate.IsPublic = adviceRequest.IsPublic;

			await _forumRepository.UpdateAdvice(adviceToUpdate);

			return Ok("Advice updated successfully");
		}
	}
}
