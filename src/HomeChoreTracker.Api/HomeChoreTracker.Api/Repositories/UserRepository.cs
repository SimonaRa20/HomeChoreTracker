using HomeChoreTracker.Api.Constants;
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
            return await _dbContext.Users.Where(x=>x.Email.Equals(inviteeEmail)).Select(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _dbContext.Users
        .Include(u => u.FinancialRecords)
        .Include(u => u.CalendarEvents)
        .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<User>> GetHomeMembers(int homeId)
        {
            return await _dbContext.UserHomes
                .Where(uh => uh.HomeId == homeId)
                .Select(uh => uh.User)
                .ToListAsync();
        }

        public async Task UpdateUser(User user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<BusyInterval>> GetUserBusyIntervals(int userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.BusyIntervals)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user.BusyIntervals.ToList();
        }

        public async Task AddBusyInterval (BusyInterval interval)
        {
            await _dbContext.BusyIntervals.AddAsync(interval);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInterval(int id)
        {
            BusyInterval interval = await _dbContext.BusyIntervals.FindAsync(id);
            if (interval != null)
            {
                _dbContext.BusyIntervals.Remove(interval);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<BusyInterval> GetBusyIntervalById(int id)
        {
            return await _dbContext.BusyIntervals.FindAsync(id);
        }

        public async Task UpdateInterval(BusyInterval interval)
        {
            _dbContext.BusyIntervals.Update(interval);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _dbContext.Users.Where(x=>x.Role.Equals(Role.User)).ToListAsync();
        }
    }
}
