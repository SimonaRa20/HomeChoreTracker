using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IGamificationRepository
    {
        Task<List<PointsHistory>> GetGamificationLevelByHomeId(int homeId);
        Task<GamificationLevel> GetGamificationLevelById(int id);
        Task<GamificationLevel> GetGamificationLevel(int level);
        Task<List<GamificationLevel>> GetGamificationLevels();
        Task AddLevel(GamificationLevel gamificationLevel);
        Task Update(GamificationLevel gamificationLevel);
        Task<PointsHistory> GetPointsHistoryByTaskId(int id);
        Task AddPointsHistory(PointsHistory points);
        Task Delete(int id);
    }
}
