using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
    public class HomeChoreBaseRepository : IHomeChoreBaseRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;

        public HomeChoreBaseRepository(HomeChoreTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<HomeChoreBase>> GetAll()
        {
            return await _dbContext.HomeChoresBases.ToListAsync();
        }


        public async Task<HomeChoreBase> GetChoreBase(int id)
        {
            return await _dbContext.HomeChoresBases.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddHomeChoreBase(HomeChoreBaseRequest homeChoreBase)
        {
            HomeChoreBase homeChore = new HomeChoreBase
            {
                Name = homeChoreBase.Name,
                ChoreType = homeChoreBase.ChoreType,
                Frequency = homeChoreBase.Frequency,
                Description = homeChoreBase.Description,
            };

            await _dbContext.HomeChoresBases.AddAsync(homeChore);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(HomeChoreBase homeChoreBase)
        {
            _dbContext.Entry(homeChoreBase).State = EntityState.Modified;
            await Save();
        }

        public async Task Delete(int id)
        {
            HomeChoreBase homeChoreBase = await _dbContext.HomeChoresBases.FindAsync(id);
            if (homeChoreBase != null)
            {
                _dbContext.HomeChoresBases.Remove(homeChoreBase);
            }
            await Save();
        }

        public async Task<List<HomeChoreBase>> GetPaginated(int skip, int take)
        {
            // Assuming you have a DbSet<HomeChoreBase> in your DbContext named "HomeChoreBases"
            List<HomeChoreBase> homeChores = await _dbContext.HomeChoresBases.OrderBy(h => h.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return homeChores;
        }
    }
}
