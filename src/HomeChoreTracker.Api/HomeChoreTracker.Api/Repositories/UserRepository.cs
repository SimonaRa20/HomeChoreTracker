using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;

        public UserRepository(HomeChoreTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetUserIdByEmail(string inviteeEmail)
        {
            return await _dbContext.Users.Where(x=>x.Email == inviteeEmail).Select(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<List<Notification>> GetUserNotifications(int userId)
        {
            return await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.Created)
                .ToListAsync();
        }

        public async Task MarkNotificationAsRead(int notificationId)
        {
            var notification = await _dbContext.Notifications.FindAsync(notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
