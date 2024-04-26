using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Avatar;
using HomeChoreTracker.Api.Contracts.Challenge;
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
    public class ChallengeController : Controller
    {
        private readonly IChallengeRepository _challengeRepository;

        public ChallengeController(IChallengeRepository challengeRepository)
        {
            _challengeRepository = challengeRepository;
        }

        [HttpPost]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> AddChallenge(ChallengeRequest challengeRequest)
        {
            var challenge = new Challenge
            {
                OpponentType = challengeRequest.OpponentType,
                HomeId = challengeRequest.HomeId,
                UserId = challengeRequest.UserId,
                OpponentHomeId = challengeRequest.OpponentHomeId,
                OpponentUserId = challengeRequest.UserId,
                ChallengeType = challengeRequest.ChallengeType,
                ChallengeCount = challengeRequest.ChallengeCount,
                Time = new TimeSpan(challengeRequest.Days, challengeRequest.Hours, challengeRequest.Minutes, challengeRequest.Seconds),
                Action = ChallengeInvitationType.None,
                ResultType = ChallengeResultType.None,
            };

            await _challengeRepository.AddChallenge(challenge);

            return Ok("Challenge added successfully");
        }

        [HttpGet("OpponentsUsers")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

                var users = await _challengeRepository.GetUsersOpponents(userId);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while get avatar: {ex.Message}");
            }
        }

        [HttpGet("UserHomes")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetUserHomes()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

                var users = await _challengeRepository.GetUserHomes(userId);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while get avatar: {ex.Message}");
            }
        }

        [HttpGet("OpponentsHomes")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetOpponentsHomes()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

                var homes = await _challengeRepository.GetOpponentsHomes(userId);

                return Ok(homes);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while get avatar: {ex.Message}");
            }
        }
    }
}
