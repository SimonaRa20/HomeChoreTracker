using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
    public class GamificationRepository : IGamificationRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;
        private readonly IUserRepository _userRepository;

        public GamificationRepository(HomeChoreTrackerDbContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public async Task<GamificationLevel> GetGamificationLevelById(int id)
        {
            return await _dbContext.GamificationLevels.Where(x => x.Id.Equals(id)).FirstAsync();
        }

        public async Task<GamificationLevel> GetGamificationLevel(int level)
        {
            return await _dbContext.GamificationLevels.Where(x => x.LevelId.Equals(level)).FirstAsync();
        }

        public async Task<List<GamificationLevel>> GetGamificationLevels()
        {
            return await _dbContext.GamificationLevels.ToListAsync();
        }

        public async Task AddLevel(GamificationLevel gamificationLevel)
        {
            await _dbContext.GamificationLevels.AddAsync(gamificationLevel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(GamificationLevel gamificationLevel)
        {
            _dbContext.Entry(gamificationLevel).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
