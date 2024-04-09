using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

		public async Task<GamificationLevel> GetMaxLevel()
		{
			return await _dbContext.GamificationLevels.OrderBy(x=>x.LevelId).LastAsync();
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

        public async Task<bool> UserHasDoneFirstTaskBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneFirstTask ?? false;
        }

        public async Task<bool> UserHasDoneFirstCleaningTaskBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneFirstCleaningTask ?? false;
        }

        public async Task<bool> UserHasDoneFirstLaundryTaskBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneFirstLaundryTask ?? false;
        }

        public async Task<bool> UserHasDoneFirstKitchenTaskBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneFirstKitchenTask ?? false;
        }

        public async Task<bool> UserHasDoneFirstBathroomTaskBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneFirstBathroomTask ?? false;
        }

        public async Task<bool> UserHasDoneFirstBedroomTaskBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneFirstBedroomTask ?? false;
        }

        public async Task<bool> UserHasDoneFirstOutdoorsTaskBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneFirstOutdoorsTask ?? false;
        }

        public async Task<bool> UserHasDoneFirstOrganizeTaskBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneFirstOrganizeTask ?? false;
        }

        public async Task<bool> UserHasEarnedPerDayFiftyPointsBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.EarnedPerDayFiftyPoints ?? false;
        }

        public async Task<bool> UserHasEarnedFiftyPointsPerDay(int userId)
        {
            DateTime todayStart = DateTime.Today;
            DateTime todayEnd = todayStart.AddDays(1).AddSeconds(-1);

            int totalPointsToday = await _dbContext.PointsHistory
                .Where(ph => ph.HomeMemberId == userId && ph.EarnedDate >= todayStart && ph.EarnedDate <= todayEnd)
                .SumAsync(ph => ph.EarnedPoints);

            return totalPointsToday >= 50;
        }

        public async Task<bool> UserHasEarnedPerDayHundredPointsBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.EarnedPerDayHundredPoints ?? false;
        }


        public async Task<bool> UserHasEarnedHundredPointsPerDay(int userId)
        {
            DateTime todayStart = DateTime.Today;
            DateTime todayEnd = todayStart.AddDays(1).AddSeconds(-1);

            int totalPointsToday = await _dbContext.PointsHistory
                .Where(ph => ph.HomeMemberId == userId && ph.EarnedDate >= todayStart && ph.EarnedDate <= todayEnd)
                .SumAsync(ph => ph.EarnedPoints);

            return totalPointsToday >= 100;
        }

        public async Task<bool> UserHasDoneFiveTaskPerWeekBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneFiveTaskPerWeek ?? false;
        }

        public async Task<bool> UserHasDoneTenTaskPerWeekBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneTenTaskPerWeek ?? false;
        }

        public async Task<bool> UserHasDoneTwentyFiveTaskPerWeekBadge(int userId)
        {
            BadgeWallet wallet = await _dbContext.BadgeWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            return wallet?.DoneTwentyFiveTaskPerWeek ?? false;
        }

        public async Task<int> GetUserBadgesCountByUserId(int userId)
        {
            BadgeWallet wallet = _dbContext.BadgeWallets.FirstOrDefault(x => x.UserId.Equals(userId));
            int trueCount = 0;
            PropertyInfo[] properties = typeof(BadgeWallet).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(bool) && (bool)property.GetValue(wallet))
                {
                    trueCount++;
                }
            }

            return trueCount;
        }

        public async Task<int> GetUserPointsByUserId(int userId)
        {
            List<PointsHistory> points = await _dbContext.PointsHistory.Where(x => x.HomeMemberId.Equals(userId)).ToListAsync();
            int sum = 0;
            foreach (var point in points)
            {
                sum += point.EarnedPoints;
            }

            return sum;
        }
    }
}
