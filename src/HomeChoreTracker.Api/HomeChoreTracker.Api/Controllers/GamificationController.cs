using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Forum;
using HomeChoreTracker.Api.Contracts.Gamification;
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
    public class GamificationController : Controller
    {
        private readonly IGamificationRepository _gamificationRepository;
        private readonly IHomeRepository _homeRepository;
        private readonly IUserRepository _userRepository;

        public GamificationController(IGamificationRepository gamificationRepository, IHomeRepository homeRepository, IUserRepository userRepository)
        {
            _gamificationRepository = gamificationRepository;
            _homeRepository = homeRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> GetGameLevels()
        {
            List<GamificationLevel> levels = await _gamificationRepository.GetGamificationLevels();
            List<GamificationLevelResponse> responses = new List<GamificationLevelResponse>();

            foreach (GamificationLevel level in levels)
            {
                GamificationLevelResponse levelResponse = new GamificationLevelResponse
                {
                    Id = level.Id,
                    LevelId = level.LevelId,
                    PointsRequired = level.PointsRequired,
                    Image = level.Image,
                };
                responses.Add(levelResponse);
            }

            return Ok(responses);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> GetGameLevel(int id)
        {
            GamificationLevel level = await _gamificationRepository.GetGamificationLevelById(id);
            GamificationLevelResponse levelResponse = new GamificationLevelResponse
            {
                Id = level.Id,
                LevelId = level.LevelId,
                PointsRequired = level.PointsRequired,
                Image = level.Image,
            };

            return Ok(levelResponse);
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> AddGamificationLevel(GamificationLevelRequest levelRequest)
        {
            if (levelRequest.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await levelRequest.Image.CopyToAsync(memoryStream);

                    var level = new GamificationLevel
                    {
                        Image = memoryStream.ToArray(),
                        LevelId = (int)levelRequest.LevelId,
                        PointsRequired = levelRequest.PointsRequired,
                    };

                    await _gamificationRepository.AddLevel(level);
                }
            }

            return Ok("Level added successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> UpdateGamificationLevel(int id, [FromForm] GamificationLevelUpdateRequest levelRequest)
        {
            try
            {
                GamificationLevel gamificationLevel = await _gamificationRepository.GetGamificationLevelById(id);

                if (gamificationLevel == null)
                {
                    return NotFound($"Gamification level with ID {gamificationLevel} not found");
                }

                if (levelRequest.Image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await levelRequest.Image.CopyToAsync(memoryStream);
                        gamificationLevel.Image = memoryStream.ToArray();
                    }
                }

                gamificationLevel.PointsRequired = levelRequest.PointsRequired;

                await _gamificationRepository.Update(gamificationLevel);

                return Ok($"Article with ID {id} updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the article: {ex.Message}");
            }
        }

        [HttpGet("ThisWeek/{homeId}")]
        [Authorize]
        public async Task<IActionResult> GetThisWeekPointsStatistic(int homeId)
        {
            List<PointsHistory> pointsHistories = await _gamificationRepository.GetHomeThisWeekPointsHistory(homeId);
            List<UserGetResponse> homers = await _homeRepository.GetHomeMembers(homeId);

            var categoryCounts = new Dictionary<string, int>();

            foreach (var member in homers)
            {
                int totalPoints = pointsHistories
                    .Where(ph => ph.HomeMemberId == member.HomeMemberId)
                    .Sum(ph => ph.EarnedPoints);

                categoryCounts.Add(member.UserName, totalPoints);
            }

            return Ok(categoryCounts);
        }

        [HttpGet("PreviousWeek/{homeId}")]
        [Authorize]
        public async Task<IActionResult> GetPreviousWeekPointsStatistic(int homeId)
        {
            List<PointsHistory> pointsHistories = await _gamificationRepository.GetHomePreviousWeekPointsHistory(homeId);
            List<UserGetResponse> homers = await _homeRepository.GetHomeMembers(homeId);

            var categoryCounts = new Dictionary<string, int>();

            foreach (var member in homers)
            {
                int totalPoints = pointsHistories
                    .Where(ph => ph.HomeMemberId == member.HomeMemberId)
                    .Sum(ph => ph.EarnedPoints);

                categoryCounts.Add(member.UserName, totalPoints);
            }

            return Ok(categoryCounts);
        }

        [HttpGet("BadgeWallet")]
        [Authorize]
        public async Task<IActionResult> GetBadgeWallet()
        {
            int id = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            BadgeWallet badgeWallet = await _gamificationRepository.GetUserBadgeWallet(id);

            BadgeWalletResponse response = new BadgeWalletResponse
            {
                Id = badgeWallet.Id,
                DoneFirstTask = badgeWallet.DoneFirstTask,
                DoneFirstCleaningTask = badgeWallet.DoneFirstCleaningTask,
                DoneFirstLaundryTask = badgeWallet.DoneFirstLaundryTask,
                DoneFirstKitchenTask = badgeWallet.DoneFirstKitchenTask,
                DoneFirstBathroomTask = badgeWallet.DoneFirstBathroomTask,
                DoneFirstBedroomTask = badgeWallet.DoneFirstBedroomTask,

                DoneFirstOutdoorsTask = badgeWallet.DoneFirstOutdoorsTask,
                DoneFirstOrganizeTask = badgeWallet.DoneFirstOrganizeTask,
                EarnedPerDayFiftyPoints = badgeWallet.EarnedPerDayFiftyPoints,
                EarnedPerDayHundredPoints = badgeWallet.EarnedPerDayHundredPoints,
                DoneFiveTaskPerWeek = badgeWallet.DoneFiveTaskPerWeek,
                DoneTenTaskPerWeek = badgeWallet.DoneTenTaskPerWeek,

                DoneTwentyFiveTaskPerWeek = badgeWallet.DoneTwentyFiveTaskPerWeek,
                CreatedTaskWasUsedOtherHome = badgeWallet.CreatedTaskWasUsedOtherHome,
                CreateFirstPurchase = badgeWallet.CreateFirstPurchase,
                CreateFirstAdvice = badgeWallet.CreateFirstAdvice,
                CreateFirstIncome = badgeWallet.CreateFirstIncome,
                CreateFirstExpense = badgeWallet.CreateFirstExpense
            };

            return Ok(response);
        }

        [HttpGet("RatingsByPoints")]
        [Authorize]
        public async Task<IActionResult> GetRatingsByPoints()
        {
            List<RatingsResponse> ratings = new List<RatingsResponse>();

            List<User> users = await _userRepository.GetAllUsers();

            foreach (User user in users)
            {
                int badges = await _gamificationRepository.GetUserBadgesCountByUserId(user.Id);
                int points = await _gamificationRepository.GetUserPointsByUserId(user.Id);

                RatingsResponse response = new RatingsResponse
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    EarnedPoints = points,
                    EarnedBadgesCount = badges,
                };

                ratings.Add(response);
            }

            var result = ratings.OrderByDescending(x=>x.EarnedPoints).ToList();
            return Ok(result);
        }

        [HttpGet("RatingsByBadges")]
        [Authorize]
        public async Task<IActionResult> GetRatingsByBadges()
        {
            List<RatingsResponse> ratings = new List<RatingsResponse>();

            List<User> users = await _userRepository.GetAllUsers();

            foreach (User user in users)
            {
                int badges = await _gamificationRepository.GetUserBadgesCountByUserId(user.Id);
                int points = await _gamificationRepository.GetUserPointsByUserId(user.Id);

                RatingsResponse response = new RatingsResponse
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    EarnedPoints = points,
                    EarnedBadgesCount = badges,
                };

                ratings.Add(response);
            }

            var result = ratings.OrderByDescending(x => x.EarnedBadgesCount).ToList();
            return Ok(result);
        }
    }
}
