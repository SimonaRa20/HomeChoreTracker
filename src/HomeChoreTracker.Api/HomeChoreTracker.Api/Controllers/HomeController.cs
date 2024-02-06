using AutoMapper;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepository;

        public HomeController(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
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

        [HttpPost("invite")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> InviteUserToHome([FromBody] InviteUserRequest inviteUserRequest)
        {
            // Get the inviter user ID from your authentication system
            int inviterUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            // Call the repository method to invite the user
            var token = await _homeRepository.InviteUserToHome(inviterUserId, inviteUserRequest.HomeId, inviteUserRequest.InviteeEmail);

            return Ok(new { InvitationToken = token });
        }

        [HttpPost("accept-invitation")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationRequest acceptInvitationRequest)
        {
            // Verify the token and perform necessary checks
            var isValid = await _homeRepository.ValidateInvitationToken(acceptInvitationRequest.InvitationToken);

            if (!isValid)
            {
                return BadRequest("Invalid or expired invitation token.");
            }

            // Get the user ID from your authentication system
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            // Call the repository method to associate the user with the home
            await _homeRepository.AssociateUserWithHome(userId, acceptInvitationRequest.InvitationToken, false);

            return Ok("Invitation accepted successfully.");
        }

        [HttpPost("respond-to-invitation")]
        public async Task<IActionResult> RespondToInvitation([FromBody] InvitationResponseRequest responseRequest)
        {
            // Get the user ID from your authentication system
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            try
            {
                await _homeRepository.AssociateUserWithHome(userId, responseRequest.InvitationToken, responseRequest.AcceptInvitation);
                return Ok("Invitation response processed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
