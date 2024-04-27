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
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var challenge = new Challenge
            {
                OpponentType = challengeRequest.OpponentType,
                ChallengeType = challengeRequest.ChallengeType,
                ChallengeCount = challengeRequest.ChallengeCount,
                DaysTime = challengeRequest.Days,
                HoursTime = challengeRequest.Hours,
                MinutesTime = challengeRequest.Minutes,
                SecondsTime = challengeRequest.Seconds,
                Action = ChallengeInvitationType.None,
                ResultType = ChallengeResultType.None,
            };

            if (challengeRequest.OpponentType.Equals(OpponentType.User))
            {
                challenge.UserId = userId;
                challenge.OpponentUserId = challengeRequest.OpponentUserId;
            }
            else
            {
                challenge.HomeId = challengeRequest.HomeId;
                challenge.OpponentHomeId = challengeRequest.OpponentHomeId;
            }

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

        [HttpGet("ReceivedChallenges")]
        [Authorize(Roles = Role.User)]
        public async Task<IActionResult> GetReceivedChallenges()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

                List<Challenge> allChallenges = await _challengeRepository.GetReceivedChallenges();
                List<ReceivedChallengeResponse> receivedChallengeResponses = new List<ReceivedChallengeResponse>();

                foreach(var challenge in allChallenges)
                {
                    ReceivedChallengeResponse receivedChallenge = new ReceivedChallengeResponse();
                    if (OpponentType.User.Equals(challenge.OpponentType) && challenge.UserId != null && challenge.OpponentUserId.Equals(userId))
                    {
                        var user = await _challengeRepository.GetUser((int)challenge.UserId);
                        var opponentUser = await _challengeRepository.GetUser((int)challenge.OpponentUserId);
                        receivedChallenge.Id = challenge.Id;
                        receivedChallenge.OpponentType = challenge.OpponentType;
                        receivedChallenge.UserName = user.UserName;
                        receivedChallenge.OpponentUserName = opponentUser.UserName;
                        receivedChallenge.ChallengeType = challenge.ChallengeType;
                        receivedChallenge.ChallengeCount = challenge.ChallengeCount;
                        receivedChallenge.Days = challenge.DaysTime;
                        receivedChallenge.Hours = challenge.HoursTime;
                        receivedChallenge.Minutes = challenge.MinutesTime;
                        receivedChallenge.Seconds = challenge.SecondsTime;

                        receivedChallengeResponses.Add(receivedChallenge);
                    }
                    else if (OpponentType.Home.Equals(challenge.OpponentType) && challenge.HomeId != null && challenge.OpponentHomeId != null)
                    {
                        var userHomes = await _challengeRepository.GetUserHomes(userId);
                        var isUserHome = userHomes.Select(x => x.Id).Contains((int)challenge.HomeId);
                        if(!isUserHome)
                        {
                            var userHome = await _challengeRepository.GetHome((int)challenge.HomeId);
                            var opponentHome = await _challengeRepository.GetHome((int)challenge.OpponentHomeId);
                            receivedChallenge.Id = challenge.Id;
                            receivedChallenge.OpponentType = challenge.OpponentType;
                            receivedChallenge.HomeTitle = userHome.Title;
                            receivedChallenge.OpponentHomeTitle = opponentHome.Title;
                            receivedChallenge.ChallengeType = challenge.ChallengeType;
                            receivedChallenge.ChallengeCount = challenge.ChallengeCount;
                            receivedChallenge.Days = challenge.DaysTime;
                            receivedChallenge.Hours = challenge.HoursTime;
                            receivedChallenge.Minutes = challenge.MinutesTime;
                            receivedChallenge.Seconds = challenge.SecondsTime;

                            receivedChallengeResponses.Add(receivedChallenge);
                        }
                    }
                }

                return Ok(receivedChallengeResponses);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while get avatar: {ex.Message}");
            }
        }

		[HttpPut("Decline/{challengeId}")]
		[Authorize(Roles = Role.User)]
		public async Task<IActionResult> DeclineChallenge(int challengeId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                var challenge = await _challengeRepository.GetChallengeById(challengeId);

                challenge.Count = 0;
                challenge.OpponentCount = 0;
                challenge.Action = ChallengeInvitationType.Decline;
                await _challengeRepository.Update(challenge);
                return Ok("Successfully declined challenge");
			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred while update challenge: {ex.Message}");
			}
		}

		[HttpPut("Accept/{challengeId}")]
		[Authorize(Roles = Role.User)]
		public async Task<IActionResult> AcceptChallenge(int challengeId)
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				var challenge = await _challengeRepository.GetChallengeById(challengeId);

				challenge.Count = 0;
				challenge.OpponentCount = 0;
				challenge.Action = ChallengeInvitationType.Accept;
                challenge.StartTime = DateTime.Now;
                challenge.EndTime = challenge.StartTime + new TimeSpan(challenge.DaysTime, challenge.HoursTime, challenge.MinutesTime, challenge.SecondsTime);
				await _challengeRepository.Update(challenge);
				return Ok("Successfully accepted challenge");
			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred while update challenge: {ex.Message}");
			}
		}

		[HttpGet("CurrentChallenges")]
		[Authorize(Roles = Role.User)]
		public async Task<IActionResult> GetCurrentChallenges()
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				List<Challenge> allChallenges = await _challengeRepository.GetCurrentChallenges();
				List<CurrentChallengeResponse> currentChallengeResponses = new List<CurrentChallengeResponse>();

				foreach (var challenge in allChallenges)
				{
					CurrentChallengeResponse receivedChallenge = new CurrentChallengeResponse();
					if (OpponentType.User.Equals(challenge.OpponentType) && challenge.UserId != null && challenge.UserId.Equals(userId) || challenge.OpponentUserId.Equals(userId))
					{
						var user = await _challengeRepository.GetUser((int)challenge.UserId);
						var opponentUser = await _challengeRepository.GetUser((int)challenge.OpponentUserId);
						receivedChallenge.Id = challenge.Id;
						receivedChallenge.OpponentType = challenge.OpponentType;
						receivedChallenge.UserName = user.UserName;
						receivedChallenge.OpponentUserName = opponentUser.UserName;
						receivedChallenge.ChallengeType = challenge.ChallengeType;
						receivedChallenge.ChallengeCount = challenge.ChallengeCount;
                        receivedChallenge.Count = (int)challenge.Count;
                        receivedChallenge.OpponentCount = (int)challenge.OpponentCount;
						receivedChallenge.EndTime = (DateTime)challenge.EndTime;

						currentChallengeResponses.Add(receivedChallenge);
					}
					else if (OpponentType.Home.Equals(challenge.OpponentType) && challenge.HomeId != null && challenge.OpponentHomeId != null)
					{
						var userHomes = await _challengeRepository.GetUserHomes(userId);
						var isUserHome = userHomes.Select(x => x.Id).Contains((int)challenge.HomeId);
						if (isUserHome)
						{
							var userHome = await _challengeRepository.GetHome((int)challenge.HomeId);
							var opponentHome = await _challengeRepository.GetHome((int)challenge.OpponentHomeId);
							receivedChallenge.Id = challenge.Id;
							receivedChallenge.OpponentType = challenge.OpponentType;
							receivedChallenge.HomeTitle = userHome.Title;
							receivedChallenge.OpponentHomeTitle = opponentHome.Title;
							receivedChallenge.ChallengeType = challenge.ChallengeType;
							receivedChallenge.ChallengeCount = challenge.ChallengeCount;
							receivedChallenge.Count = (int)challenge.Count;
							receivedChallenge.OpponentCount = (int)challenge.OpponentCount;
                            receivedChallenge.EndTime = (DateTime)challenge.EndTime;

							currentChallengeResponses.Add(receivedChallenge);
						}
					}
				}

				return Ok(currentChallengeResponses);
			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred while get avatar: {ex.Message}");
			}
		}

		[HttpGet("HistoryChallenges")]
		[Authorize(Roles = Role.User)]
		public async Task<IActionResult> GetHistoryChallenges()
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				List<Challenge> allChallenges = await _challengeRepository.GetHistoryChallenges();
				List<HistoryChallengeResponse> currentChallengeResponses = new List<HistoryChallengeResponse>();

				foreach (var challenge in allChallenges)
				{
					HistoryChallengeResponse receivedChallenge = new HistoryChallengeResponse();
					if (OpponentType.User.Equals(challenge.OpponentType) && challenge.UserId != null && challenge.UserId.Equals(userId) || challenge.OpponentUserId.Equals(userId))
					{
						var user = await _challengeRepository.GetUser((int)challenge.UserId);
						var opponentUser = await _challengeRepository.GetUser((int)challenge.OpponentUserId);
						receivedChallenge.Id = challenge.Id;
						receivedChallenge.OpponentType = challenge.OpponentType;
						receivedChallenge.UserName = user.UserName;
						receivedChallenge.OpponentUserName = opponentUser.UserName;
						receivedChallenge.ChallengeType = challenge.ChallengeType;
						receivedChallenge.ChallengeCount = challenge.ChallengeCount;
						receivedChallenge.Count = (int)challenge.Count;
						receivedChallenge.OpponentCount = (int)challenge.OpponentCount;
						receivedChallenge.ResultType = challenge.ResultType;

						currentChallengeResponses.Add(receivedChallenge);
					}
					else if (OpponentType.Home.Equals(challenge.OpponentType) && challenge.HomeId != null && challenge.OpponentHomeId != null)
					{
						var userHomes = await _challengeRepository.GetUserHomes(userId);
						var isUserHome = userHomes.Select(x => x.Id).Contains((int)challenge.HomeId);
						if (isUserHome)
						{
							var userHome = await _challengeRepository.GetHome((int)challenge.HomeId);
							var opponentHome = await _challengeRepository.GetHome((int)challenge.OpponentHomeId);
							receivedChallenge.Id = challenge.Id;
							receivedChallenge.OpponentType = challenge.OpponentType;
							receivedChallenge.HomeTitle = userHome.Title;
							receivedChallenge.OpponentHomeTitle = opponentHome.Title;
							receivedChallenge.ChallengeType = challenge.ChallengeType;
							receivedChallenge.ChallengeCount = challenge.ChallengeCount;
							receivedChallenge.Count = (int)challenge.Count;
							receivedChallenge.OpponentCount = (int)challenge.OpponentCount;
							receivedChallenge.ResultType = challenge.ResultType;

							currentChallengeResponses.Add(receivedChallenge);
						}
					}
				}

				return Ok(currentChallengeResponses);
			}
			catch (Exception ex)
			{
				return BadRequest($"An error occurred while get avatar: {ex.Message}");
			}
		}
	}
}
