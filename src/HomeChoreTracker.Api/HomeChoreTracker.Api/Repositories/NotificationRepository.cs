using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;

        public NotificationRepository(HomeChoreTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateNotification(Notification notification)
        {
            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetNotifications(int userId)
        {
            return await _dbContext.Notifications.Where(x => x.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<List<Notification>> GetNotReadNotifications(int userId)
        {
            return await _dbContext.Notifications.Where(x => x.UserId.Equals(userId) && !x.IsRead).ToListAsync();
        }

        public async Task Update(Notification notification)
        {
            _dbContext.Entry(notification).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
