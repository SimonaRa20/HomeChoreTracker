using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using DayOfWeek = HomeChoreTracker.Api.Constants.DayOfWeek;

namespace HomeChoreTracker.Api.Repositories
{
    public class HomeChoreRepository : IHomeChoreRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;

        public HomeChoreRepository(HomeChoreTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddHomeChoreBase(HomeChoreBase homeChoreBase, int homeId)
        {
            HomeChoreTask homeChore = new HomeChoreTask
            {
                Name = homeChoreBase.Name,
                ChoreType = homeChoreBase.ChoreType,
                Description = homeChoreBase.Description,
                Points = homeChoreBase.Points,
                Time = homeChoreBase.Time,
                LevelType = homeChoreBase.LevelType,
                Interval = homeChoreBase.Interval,
                Unit = homeChoreBase.Unit,
                DaysOfWeek = homeChoreBase.DaysOfWeek,
                DayOfMonth = homeChoreBase.DayOfMonth,
                MonthlyRepeatType = homeChoreBase.MonthlyRepeatType,
                IsActive = true,
                HomeId = homeId,
            };

            await _dbContext.HomeChoreTasks.AddAsync(homeChore);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateHomeChore(HomeChoreRequest homeChoreRequest)
        {
            List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();
            foreach (int day in homeChoreRequest.DaysOfWeek)
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


            HomeChoreTask homeChore = new HomeChoreTask
            {
                Name = homeChoreRequest.Name,
                ChoreType = homeChoreRequest.ChoreType,
                Description = homeChoreRequest.Description,
                Points = homeChoreRequest.Points,
                Time = homeChoreRequest.Time,
                LevelType = homeChoreRequest.LevelType,
                Interval = homeChoreRequest.Interval,
                Unit = homeChoreRequest.Unit,
                DaysOfWeek = dayOfWeeks,
                DayOfMonth = homeChoreRequest.DayOfMonth,
                MonthlyRepeatType = homeChoreRequest.MonthlyRepeatType,
                IsActive = true,
                HomeId = homeChoreRequest.HomeId,
            };

            await _dbContext.HomeChoreTasks.AddAsync(homeChore);
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
