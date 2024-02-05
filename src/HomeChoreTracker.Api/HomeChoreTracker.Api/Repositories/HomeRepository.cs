using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;

        public HomeRepository(HomeChoreTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateHome(HomeRequest homeRequest, int userId)
        {
            Home home = new Home
            {
                Title = homeRequest.Title,
            };

            await _dbContext.Homes.AddAsync(home);
            await _dbContext.SaveChangesAsync();

            var userHome = new UserHomes
            {
                UserId = userId,
                HomeId = home.Id,
                HomeRole = HomeRole.HomeAdmin,
            };

            _dbContext.UserHomes.Add(userHome);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Home>> GetAll(int userId)
        {
            return await _dbContext.UserHomes.Where(x => x.UserId == userId).Select(h => h.Home).ToListAsync();
        }
    }
}
