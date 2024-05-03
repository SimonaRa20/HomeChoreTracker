using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
using DayOfWeek = HomeChoreTracker.Api.Constants.DayOfWeek;

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

        public async Task<HomeChoreBase> AddHomeChoreBase(HomeChoreBaseRequest homeChoreBase)
        {
            List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();

            if(homeChoreBase.DaysOfWeek != null)
            {
                foreach (int day in homeChoreBase.DaysOfWeek)
                {
                    if (day == 0)
                    {
                        dayOfWeeks.Add(DayOfWeek.Default);
                    }
                    if (day == 1)
                    {
                        dayOfWeeks.Add(DayOfWeek.Monday);
                    }
                    if (day == 2)
                    {
                        dayOfWeeks.Add(DayOfWeek.Tuesday);
                    }
                    if (day == 3)
                    {
                        dayOfWeeks.Add(DayOfWeek.Wednesday);
                    }
                    if (day == 4)
                    {
                        dayOfWeeks.Add(DayOfWeek.Thursday);
                    }
                    if (day == 5)
                    {
                        dayOfWeeks.Add(DayOfWeek.Friday);
                    }
                    if (day == 6)
                    {
                        dayOfWeeks.Add(DayOfWeek.Saturday);
                    }
                    if (day == 7)
                    {
                        dayOfWeeks.Add(DayOfWeek.Sunday);
                    }
                }
            }

            else if(homeChoreBase.Unit == Constants.RepeatUnit.Week && homeChoreBase.DaysOfWeek == null)
            {
                dayOfWeeks.Add(DayOfWeek.Default);
            }


            HomeChoreBase homeChore = new HomeChoreBase
            {
                Name = homeChoreBase.Name,
                ChoreType = homeChoreBase.ChoreType,
                Description = homeChoreBase.Description,
                Points = homeChoreBase.Points,
                HoursTime = homeChoreBase.HoursTime,
                MinutesTime = homeChoreBase.MinutesTime,
                LevelType = homeChoreBase.LevelType,
                Interval = homeChoreBase.Interval,
                Unit = homeChoreBase.Unit,
                DaysOfWeek = dayOfWeeks,
                DayOfMonth = homeChoreBase.DayOfMonth,
                MonthlyRepeatType = homeChoreBase.MonthlyRepeatType,
            };

            await _dbContext.HomeChoresBases.AddAsync(homeChore);
            await _dbContext.SaveChangesAsync();
            return homeChore;
        }

        public async Task Update(HomeChoreBase homeChoreBase)
        {
            _dbContext.Entry(homeChoreBase).State = EntityState.Modified;
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
            List<HomeChoreBase> homeChores = await _dbContext.HomeChoresBases.OrderBy(h => h.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return homeChores;
        }
    }
}
