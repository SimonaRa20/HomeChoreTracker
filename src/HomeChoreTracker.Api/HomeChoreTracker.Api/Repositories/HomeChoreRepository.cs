using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
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
                WasEarnedPoints = false,
            };

            await _dbContext.HomeChoreTasks.AddAsync(homeChore);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddTaskAssignment(TaskAssignment taskAssignment)
        {
            await _dbContext.TaskAssignments.AddAsync(taskAssignment);
        }

        public async Task <HomeChoreTask>CreateHomeChore(HomeChoreRequest homeChoreRequest)
        {
            List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();
            if(homeChoreRequest.DaysOfWeek != null)
            {
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
                StartDate = homeChoreRequest.StartDate,
                EndDate = homeChoreRequest.EndDate,
                IsActive = true,
                HomeId = homeChoreRequest.HomeId,
                WasEarnedPoints = false,
            };

            await _dbContext.HomeChoreTasks.AddAsync(homeChore);
            return homeChore;
        }

        public async Task Delete(int id)
        {
            HomeChoreTask homeChore = await _dbContext.HomeChoreTasks.FindAsync(id);
            if (homeChore != null)
            {
                _dbContext.HomeChoreTasks.Remove(homeChore);
            }
        }

        public async Task<HomeChoreTask> Get(int id)
        {
            return await _dbContext.HomeChoreTasks.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<TaskAssignment> GetTaskAssigment(int id)
        {
            return await _dbContext.TaskAssignments.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<HomeChoreTask>> GetAll(int id)
        {
            return await _dbContext.HomeChoreTasks.Where(x=>x.HomeId.Equals(id)).ToListAsync();
        }

        public async Task<List<TaskAssignment>> GetCalendar(int id)
        {
            return await _dbContext.TaskAssignments.Where(x => x.HomeId.Equals(id)).ToListAsync();
        }

        public async Task<bool> CheckOrHomeChoreWasAssigned(int id) // id - send task id
        {
            List<TaskAssignment> tasks = await _dbContext.TaskAssignments.Where(x => x.TaskId.Equals(id)).ToListAsync();
            bool flag = false;

            foreach (var task in tasks)
            {
                if(task.TaskId.Equals(id) && task.HomeMemberId != null)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        public async Task DeleteAssignedTasks(int id) // id - send task id
        {
            List<TaskAssignment> tasks = await _dbContext.TaskAssignments.Where(x => x.TaskId.Equals(id)).ToListAsync();

            foreach (var task in tasks)
            {
                if (task.TaskId.Equals(id) && task.HomeMemberId == null)
                {
                    if (task.StartDate > DateTime.Now)
                    {
                        _dbContext.TaskAssignments.Remove(task);
                    }
                    else
                    {
                        task.EndDate = DateTime.Now;
                        _dbContext.Entry(task).State = EntityState.Modified;
                    }
                }
            }
        }

        public async Task DeleteNotAssignedTasks(int id) // id - send task id
        {
            List<TaskAssignment> tasks = await _dbContext.TaskAssignments.Where(x => x.TaskId.Equals(id)).ToListAsync();

            foreach (var task in tasks)
            {
                _dbContext.TaskAssignments.Remove(task);
            }
        }

        public async Task<List<TaskSchedule>> GetTaskSchedule(int id)
        {
            return await _dbContext.TaskSchedules.Where(x => x.TaskId.Equals(id)).ToListAsync();
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task SetHomeChoreDates(TaskSchedule taskSchedule)
        {
            await _dbContext.TaskSchedules.AddAsync(taskSchedule);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(HomeChoreTask homeChoreTask)
        {
            _dbContext.Entry(homeChoreTask).State = EntityState.Modified;
        }

        public async Task UpdateTaskAssignment(TaskAssignment assignment)
        {
            _dbContext.Entry(assignment).State = EntityState.Modified;
        }
    }
}
