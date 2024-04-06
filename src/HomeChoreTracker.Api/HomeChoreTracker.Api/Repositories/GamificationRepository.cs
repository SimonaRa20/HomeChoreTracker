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

        public async Task<List<PointsHistory>> GetPointsHistoryByHomeId(int homeId)
        {
            return await _dbContext.PointsHistory.Where(x => x.HomeId.Equals(homeId)).ToListAsync();
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

        public async Task<PointsHistory> GetPointsHistoryByTaskId(int id)
        {
            return await _dbContext.PointsHistory.Where(x => x.TaskId.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task AddPointsHistory(PointsHistory points)
        {
            await _dbContext.PointsHistory.AddAsync(points);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            PointsHistory points = await _dbContext.PointsHistory.FindAsync(id);
            if (points != null)
            {
                _dbContext.PointsHistory.Remove(points);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PointsHistory>> GetHomeThisWeekPointsHistory(int homeId)
        {
            DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

            DateTime endOfWeek = startOfWeek.AddDays(6);

            return await _dbContext.PointsHistory
                .Where(x => x.TaskId == homeId && x.EarnedDate >= startOfWeek && x.EarnedDate <= endOfWeek)
                .ToListAsync();
        }


        public async Task<List<PointsHistory>> GetHomePreviousWeekPointsHistory(int homeId)
        {
            DateTime startOfPreviousWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1).AddDays(-7);

            DateTime endOfPreviousWeek = startOfPreviousWeek.AddDays(6);

            return await _dbContext.PointsHistory
                .Where(x => x.TaskId == homeId && x.EarnedDate >= startOfPreviousWeek && x.EarnedDate <= endOfPreviousWeek)
                .ToListAsync();
        }

        public async Task<BadgeWallet> GetUserBadgeWallet(int userId)
        {
            return await _dbContext.BadgeWallets.Where(x => x.UserId.Equals(userId)).FirstOrDefaultAsync();
        }

        public async Task UpdateBadgeWallet(BadgeWallet wallet)
        {
            _dbContext.Entry(wallet).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UserHasCreateFirstIncomeBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.CreateFirstIncome ?? false;
        }

        public async Task<bool> UserHasCreateFirstExpenseBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.CreateFirstExpense ?? false;
        }

        public async Task<bool> UserHasCreateFirstAdviceBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.CreateFirstAdvice ?? false;
        }

        public async Task<bool> UserHasCreateFirstPurchaseBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.CreateFirstPurchase ?? false;
        }

        public async Task<bool> UserHasCreatedTaskWasUsedOtherHomeBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.CreatedTaskWasUsedOtherHome ?? false;
        }
    }
}
