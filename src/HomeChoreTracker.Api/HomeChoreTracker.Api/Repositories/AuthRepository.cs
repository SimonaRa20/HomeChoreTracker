using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;

        public AuthRepository(HomeChoreTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task AddUser(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

		public async Task<User> GetUserById(int id)
		{
			return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
