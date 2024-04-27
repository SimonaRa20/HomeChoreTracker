using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
    public class ChallengeRepository : IChallengeRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;

        public ChallengeRepository(HomeChoreTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddChallenge(Challenge challenge)
        {
            await _dbContext.AddAsync(challenge);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsersOpponents(int userId)
        {
            return await _dbContext.Users.Where(x => !x.Id.Equals(userId) && x.Role.Equals("User")).ToListAsync();
        }

        public async Task<List<Home>> GetOpponentsHomes(int userId)
        {
            var userHomes = await _dbContext.UserHomes.Where(x => x.UserId == userId).Select(h => h.Home).ToListAsync();
            var opponentHomes = await _dbContext.UserHomes.Select(h => h.Home).Where(x=>!userHomes.Contains(x)).ToListAsync();


            return opponentHomes;
        }

        public async Task<List<Home>> GetUserHomes(int userId)
        {
            return await _dbContext.UserHomes.Where(x => x.UserId == userId).Select(h => h.Home).ToListAsync();
        }

        public async Task<Home> GetHome(int homeId)
        {
            return await _dbContext.Homes.Where(x => x.Id == homeId).FirstOrDefaultAsync();
        }

        public async Task<User> GetUser(int userId)
        {
            return await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<List<Challenge>> GetReceivedChallenges()
        {
            return await _dbContext.Challenges.Where(x => x.Action.Equals(ChallengeInvitationType.None)).ToListAsync();
        }
    }
}
