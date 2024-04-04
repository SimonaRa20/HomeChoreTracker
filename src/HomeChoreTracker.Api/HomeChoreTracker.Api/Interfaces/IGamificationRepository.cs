using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IGamificationRepository
    {
        Task<GamificationLevel> GetGamificationLevelById(int id);
        Task<GamificationLevel> GetGamificationLevel(int level);
        Task<List<GamificationLevel>> GetGamificationLevels();
        Task AddLevel(GamificationLevel gamificationLevel);
        Task Update(GamificationLevel gamificationLevel);
    }
}
