using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Avatar;
using HomeChoreTracker.Api.Contracts.Gamification;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace HomeChoreTracker.Api.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AvatarController : Controller
	{
		private readonly IAvatarRepository _avatarRepository;

		public AvatarController(IAvatarRepository avatarRepository)
		{
			_avatarRepository = avatarRepository;
		}

		[HttpGet]
		[Authorize(Roles = Role.Admin)]
		public async Task<IActionResult> GetAvatars()
		{
			List<Avatar> avatars = await _avatarRepository.GetAll();
			List<AvatarResponse> responses = new List<AvatarResponse>();

			foreach (Avatar avatar in avatars)
			{
				AvatarResponse avatarResponse = new AvatarResponse
				{
					Id = avatar.Id,
					AvatarType = avatar.AvatarType,
					Image = avatar.Image,
				};
				responses.Add(avatarResponse);
			}

			return Ok(responses);
		}

		[HttpGet("GetAvatars")]
		[Authorize(Roles = Role.User)]
		public async Task<IActionResult> GetUserAvatars()
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			List<Avatar> avatars = await _avatarRepository.GetAll();
			List<AvatarSelectionResponse> responses = new List<AvatarSelectionResponse>();
			List<AvatarPurchase> purchases = await _avatarRepository.GetPurchaseAvatars(userId);
			foreach (Avatar avatar in avatars)
			{
				if (avatar.AvatarType.Equals(AvatarType.Basic))
				{
					AvatarSelectionResponse avatarResponse = new AvatarSelectionResponse
					{
						Id = avatar.Id,
						AvatarType = avatar.AvatarType,
						Image = avatar.Image,
						IsPurchased = true,
					};
					responses.Add(avatarResponse);
				}
				else
				{
					var purchase = purchases.Where(x => x.AvatarId.Equals(avatar.Id)).FirstOrDefault();
					if (purchases.Count > 0 && purchase != null)
					{
						AvatarSelectionResponse avatarResponse = new AvatarSelectionResponse
						{
							Id = avatar.Id,
							AvatarType = avatar.AvatarType,
							Image = avatar.Image,
							IsPurchased = true,
						};
						responses.Add(avatarResponse);
					}
					else
					{
						AvatarSelectionResponse avatarResponse = new AvatarSelectionResponse
						{
							Id = avatar.Id,
							AvatarType = avatar.AvatarType,
							Image = avatar.Image,
							IsPurchased = false,
						};
						responses.Add(avatarResponse);
					}
				}
			}

			return Ok(responses);
		}

		[HttpGet("GetUserAvatar")]
		[Authorize(Roles = Role.User)]
		public async Task<IActionResult> GetUserAvatar()
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			var avatar = await _avatarRepository.GetUserAvatar(userId);

			return Ok(avatar);
		}

		[HttpPut("Set/{id}")]
		[Authorize(Roles = Role.User)]
		public async Task<IActionResult> GetUserSetAvatar(int id)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			var avatar = await _avatarRepository.GetAvatar(id);

			await _avatarRepository.SetAvatar(userId, avatar);

			return Ok("Successfully updated");
		}

		[HttpPut("Buy/{id}")]
		[Authorize(Roles = Role.User)]
		public async Task<IActionResult> GetUserBuyAvatar(int id)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			var avatar = await _avatarRepository.GetAvatar(id);
			await _avatarRepository.SetAvatar(userId, avatar);
			await _avatarRepository.CountThePoints(userId, avatar);
			return Ok(avatar);
		}

		[HttpGet("UserPoints")]
		[Authorize]
		public async Task<IActionResult> GetUserPoints()
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				var pointsHistory = await _avatarRepository.GetPointsHistoryByUserId(userId);

				int points = 0;

				foreach (var history in pointsHistory)
				{
					points += history.EarnedPoints;
				}

				return Ok(points);
			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred while get points: {ex.Message}");
			}
		}

		[HttpGet("{id}")]
		[Authorize]
		public async Task<IActionResult> GetAvatar(int id)
		{
			try
			{
				var avatar = await _avatarRepository.GetAvatar(id);

				if (avatar == null)
				{
					return NotFound($"Avatar with ID {id} not found");
				}

				return Ok(avatar);
			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred while get avatar: {ex.Message}");
			}
		}

		[HttpPost]
		[Authorize(Roles = Role.Admin)]
		public async Task<IActionResult> AddAvatar(AvatarRequest avatarRequest)
		{
			if (avatarRequest.Image != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await avatarRequest.Image.CopyToAsync(memoryStream);

					var avatar = new Avatar
					{
						AvatarType = Enum.Parse<AvatarType>(avatarRequest.AvatarType),
						Image = memoryStream.ToArray()
					};

					await _avatarRepository.AddAvatar(avatar);
				}
			}

			return Ok("Avatar added successfully");
		}


		[HttpPut("{id}")]
		[Authorize(Roles = Role.Admin)]
		public async Task<IActionResult> UpdateAvatar(int id, [FromForm] AvatarUpdateRequest avatarRequest)
		{
			try
			{
				Avatar avatar = await _avatarRepository.GetAvatar(id);

				if (avatar == null)
				{
					return NotFound($"Avatar with ID {avatar} not found");
				}

				if (avatarRequest.Image != null)
				{
					using (var memoryStream = new MemoryStream())
					{
						await avatarRequest.Image.CopyToAsync(memoryStream);
						avatar.Image = memoryStream.ToArray();
					}
				}

				avatar.AvatarType = avatarRequest.AvatarType;

				await _avatarRepository.Update(avatar);

				return Ok($"Avatar with ID {id} updated successfully");
			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred while updating the avatar: {ex.Message}");
			}
		}
	}
}
