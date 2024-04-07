using DocumentFormat.OpenXml.Wordprocessing;
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

        public async Task <HomeChoreTask>CreateHomeChore(HomeChoreRequest homeChoreRequest, int userId, int? homeId)
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

            if (homeChoreRequest.IsPublic)
            {
                HomeChoreBase homeChoreBase = new HomeChoreBase
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
                    UserId = userId,
                    HomeId = (int)homeId
                };

                await _dbContext.HomeChoresBases.AddAsync(homeChoreBase);
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

        public async Task<List<TaskAssignment>> GetDoneTaskAssigment(int id)
        {
            return await _dbContext.TaskAssignments.Where(x => x.HomeId.Equals(id) && x.IsDone).ToListAsync();
        }

        public async Task<List<TaskAssignment>> GetDoneTaskAssigments(int userId)
        {
            return await _dbContext.TaskAssignments.Where(x => x.HomeMemberId.Equals(userId) && x.IsDone).ToListAsync();
        }

        public async Task<List<TaskAssignment>> GetAssignedTasks(int userId)
        {
            return await _dbContext.TaskAssignments.Where(x => x.HomeMemberId.Equals(userId)).ToListAsync();
        }


        public async Task<int> GetTotalPointsAssigned(int memberId)
        {
            var completedAssignments = await _dbContext.TaskAssignments
        .Where(assignment => assignment.HomeMemberId == memberId)
        .ToListAsync();

            int totalPoints = 0;

            foreach (var assignment in completedAssignments)
            {
                var task = await _dbContext.HomeChoreTasks.FindAsync(assignment.TaskId);
                if (task != null)
                {
                    totalPoints += task.Points;
                }
            }

            return totalPoints;
        }

        public async Task<List<HomeChoreTask>> GetAll(int id)
        {
            return await _dbContext.HomeChoreTasks.Where(x=>x.HomeId.Equals(id)).ToListAsync();
        }

        public async Task<List<TaskAssignment>> GetCalendar(int id)
        {
            return await _dbContext.TaskAssignments.Where(x => x.HomeId.Equals(id)).ToListAsync();
        }

        public async Task<bool> CheckOrHomeChoreWasAssigned(int id)
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

        public async Task<List<TaskAssignment>> GetUnassignedTasks(int homeId)
        {
            var unassignedTasks = await _dbContext.TaskAssignments
                .Where(assignment => assignment.HomeId == homeId && assignment.HomeMemberId == null)
                .ToListAsync();

            return unassignedTasks;
        }

        public async Task DeleteAssignedTasks(int id)
        {
            List<TaskAssignment> tasks = await _dbContext.TaskAssignments.Where(x => x.TaskId.Equals(id)).ToListAsync();

            foreach (var task in tasks)
            {
                if (task.TaskId.Equals(id) && task.HomeMemberId == null)
                {
                    _dbContext.TaskAssignments.Remove(task);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteNotAssignedTasks(int id)
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
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TaskAssignment>> GetHomeChoresUserCalendar(int id)
        {
            return await _dbContext.TaskAssignments.Where(x => x.HomeMemberId.Equals(id)).ToListAsync();
        }

        public async Task<TaskAssignment> GetLastAssigmentTask(int id)
        {
            return await _dbContext.TaskAssignments.Where(x => x.TaskId.Equals(id)).OrderByDescending(x => x.EndDate).FirstAsync();
        }

        public async Task<bool> VoteArtical(int taskId, int userId, int voteValue)
        {
            var task = await _dbContext.TaskAssignments
                .Include(x => x.TaskVotes)
                .FirstOrDefaultAsync(x => x.Id == taskId);

            if (task == null)
            {
                return false;
            }

            var existingVote = task.TaskVotes.FirstOrDefault(x => x.UserId == userId);

            if (existingVote != null)
            {
                existingVote.Value = voteValue;
            }
            else
            {
                var newVote = new TaskVote
                {
                    Value = voteValue,
                    UserId = userId,
                    TaskAssignmentId = task.Id,
                    TaskAssignment = task
                };
                task.TaskVotes.Add(newVote);
            }

            await Save();
            return true;
        }

        public async Task<int> GetTotalVotes(int taskId)
        {
            return await _dbContext.TaskVotes.Where(x => x.TaskAssignmentId == taskId).SumAsync(v => v.Value);
        }
    }
}
