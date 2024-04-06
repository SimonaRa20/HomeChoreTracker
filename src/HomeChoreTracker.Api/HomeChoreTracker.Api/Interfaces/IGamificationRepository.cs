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
    }
}
