using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Avatar;
using HomeChoreTracker.Api.Contracts.Gamification;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

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


		[HttpGet("{id}")]
		[Authorize]
		public async Task<IActionResult> GetAdvice(int id)
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
