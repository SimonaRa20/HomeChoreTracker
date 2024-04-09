using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IGamificationRepository
    {
        Task<List<PointsHistory>> GetPointsHistoryByHomeId(int homeId);
        Task<GamificationLevel> GetGamificationLevelById(int id);
        Task<GamificationLevel> GetGamificationLevel(int level);
        Task<List<GamificationLevel>> GetGamificationLevels();
        Task AddLevel(GamificationLevel gamificationLevel);
        Task Update(GamificationLevel gamificationLevel);
        Task<PointsHistory> GetPointsHistoryByTaskId(int id);
        Task AddPointsHistory(PointsHistory points);
        Task Delete(int id);
        Task<List<PointsHistory>> GetHomeThisWeekPointsHistory(int homeId);
        Task<List<PointsHistory>> GetHomePreviousWeekPointsHistory(int homeId);
        Task<BadgeWallet> GetUserBadgeWallet(int userId);
        Task UpdateBadgeWallet(BadgeWallet wallet);
        Task<bool> UserHasCreateFirstIncomeBadge(int userId);
        Task<bool> UserHasCreateFirstExpenseBadge(int userId);
        Task<bool> UserHasCreateFirstAdviceBadge(int userId);
        Task<bool> UserHasCreateFirstPurchaseBadge(int userId);
        Task<bool> UserHasCreatedTaskWasUsedOtherHomeBadge(int userId);
        Task<bool> UserHasDoneFirstTaskBadge(int userId);
        Task<bool> UserHasDoneFirstCleaningTaskBadge(int userId);
        Task<bool> UserHasDoneFirstLaundryTaskBadge(int userId);
        Task<bool> UserHasDoneFirstKitchenTaskBadge(int userId);
        Task<bool> UserHasDoneFirstBathroomTaskBadge(int userId);
        Task<bool> UserHasDoneFirstBedroomTaskBadge(int userId);
        Task<bool> UserHasDoneFirstOutdoorsTaskBadge(int userId);
        Task<bool> UserHasDoneFirstOrganizeTaskBadge(int userId);
        Task<bool> UserHasEarnedFiftyPointsPerDay(int userId);
        Task<bool> UserHasEarnedPerDayFiftyPointsBadge(int userId);
        Task<bool> UserHasEarnedHundredPointsPerDay(int userId);
        Task<bool> UserHasEarnedPerDayHundredPointsBadge(int userId);
        Task<bool> UserHasDoneFiveTaskPerWeekBadge(int userId);
        Task<bool> UserHasDoneTenTaskPerWeekBadge(int userId);
        Task<bool> UserHasDoneTwentyFiveTaskPerWeekBadge(int userId);
        Task<int> GetUserBadgesCountByUserId(int userId);
        Task<int> GetUserPointsByUserId(int userId);
        Task<GamificationLevel> GetMaxLevel();

	}
}
