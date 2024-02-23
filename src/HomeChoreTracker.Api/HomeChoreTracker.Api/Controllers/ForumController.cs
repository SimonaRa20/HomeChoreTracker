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
	public class ForumController : ControllerBase
	{
		private readonly IForumRepository _forumRepository;
		private readonly IUserRepository _userRepository;

        public ForumController(IForumRepository forumRepository, IUserRepository userRepository)
        {
            _forumRepository = forumRepository;
			_userRepository = userRepository;
        }

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> AddIncome(AdviceRequest adviceRequest)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
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
		public async Task<IActionResult> GetAdvicePermission(int id)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			bool hasPermission = await _forumRepository.HasPermission(id,userId);
			return Ok(hasPermission);
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

			if (adviceToUpdate == null)
			{
				return NotFound($"Advice with ID {id} not found");
			}

			if (adviceToUpdate.UserId != userId)
			{
				return Unauthorized("You do not have permission to update this advice");
			}

			adviceToUpdate.Title = adviceRequest.Title;
			adviceToUpdate.Type = adviceRequest.Type;
			adviceToUpdate.Description = adviceRequest.Description;
			adviceToUpdate.IsPublic = adviceRequest.IsPublic;

			await _forumRepository.UpdateAdvice(adviceToUpdate);

			return Ok("Advice updated successfully");
		}
	}
}
