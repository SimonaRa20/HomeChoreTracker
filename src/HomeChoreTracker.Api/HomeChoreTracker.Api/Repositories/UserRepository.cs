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
        public async Task<User> GetUserById(int id)
        {
            return await _dbContext.Users
        .Include(u => u.Incomes)
        .Include(u => u.Expenses)
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

    }
}
