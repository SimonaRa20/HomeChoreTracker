using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using DayOfWeek = HomeChoreTracker.Api.Constants.DayOfWeek;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeChoreController : Controller
    {
        private readonly IHomeChoreRepository _homeChoreRepository;
        private readonly IHomeRepository _homeRepository;
        private readonly IHomeChoreBaseRepository _homeChoreBaseRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IGamificationRepository _gamificationRepository;
        private readonly IMapper _mapper;

        public HomeChoreController(IHomeChoreRepository homeChoreRepository, IGamificationRepository gamificationRepository, IMapper mapper, IUserRepository userRepository, INotificationRepository notificationRepository, IHomeChoreBaseRepository homeChoreBaseRepository, IHomeRepository homeRepository)
        {
            _homeChoreRepository = homeChoreRepository;
            _mapper = mapper;
            _homeChoreBaseRepository = homeChoreBaseRepository;
            _homeRepository  = homeRepository;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _gamificationRepository = gamificationRepository;
        }

        [HttpPost("{homeId}")]
        [Authorize]
        public async Task<IActionResult> AddHomeChore(int homeId, int taskId)
        {
            try
            {
                int id = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                var homeChoreBase = await _homeChoreBaseRepository.GetChoreBase(taskId);

                await _homeChoreRepository.AddHomeChoreBase(homeChoreBase, homeId);

                var users = await _homeRepository.GetHomeMembers(homeId);
               

                if(homeChoreBase.UserId != null)
                {
                    bool newFamily = false;

                    foreach (var familyMember in users)
                    {
                        if (familyMember.HomeMemberId.Equals(id) && !homeChoreBase.UserId.Equals(id))
                        {
                            newFamily = true;
                        }
                    }

                    if(newFamily)
                    {
                        var user = await _userRepository.GetUserById((int)homeChoreBase.UserId);

                        PointsHistory pointsHistory = new PointsHistory
                        {
                            EarnedPoints = 5,
                            HomeMemberId = (int)homeChoreBase.UserId,
                            TaskId = (int)taskId,
                            HomeId = (int)homeChoreBase.HomeId,
                            Text = $"{user.UserName} created '{homeChoreBase.Name}' task was used.",
                            EarnedDate = DateTime.Now,
                        };

                        await _gamificationRepository.AddPointsHistory(pointsHistory);

                        Notification notification = new Notification
                        {
                            Title = $"Your created '{homeChoreBase.Name}' task was used. You earned 5 points.",
                            IsRead = false,
                            Time = DateTime.Now,
                            UserId = (int)homeChoreBase.UserId,
                            User = user,
                        };

                        await _notificationRepository.CreateNotification(notification);

                        var hasBadge = await _gamificationRepository.UserHasCreatedTaskWasUsedOtherHomeBadge(id);
                        if (!hasBadge)
                        {
                            BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(id);
                            wallet.CreateFirstExpense = true;
                            await _gamificationRepository.UpdateBadgeWallet(wallet);

                            Notification noti = new Notification
                            {
                                Title = $"You earned badge 'Create first purchase'",
                                IsRead = false,
                                Time = DateTime.Now,
                                UserId = (int)id,
                                User = user,
                            };

                            await _notificationRepository.CreateNotification(noti);
                        }
                    }
                }

                return Ok($"Home chore was added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the home chore: {ex.Message}");
            }
        }

        [HttpPost("add/{homeId}")]
        public async Task<IActionResult> CreateHomeChoreBase(int homeId, HomeChoreRequest homeChoreBaseRequest)
        {
            int id = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            HomeChoreTask task = await _homeChoreRepository.CreateHomeChore(homeChoreBaseRequest, id, homeId);

            await _homeChoreRepository.Save();
            if (task == null)
            {
                return NotFound("No home chore found");
            }


            await SetHomeChoreDates(task);
            return Ok("Home chore base created successfully");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetHomeChoresBase(int id)
        {
            try
            {
                var homeChore = await _homeChoreRepository.GetAll(id);

                return Ok(homeChore);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching home chore bases: {ex.Message}");
            }
        }

        [HttpDelete("{taskId}")]
        [Authorize]
        public async Task<IActionResult> DeleteHomeChore(int taskId)
        {
            try
            {
                TaskAssignment task = await _homeChoreRepository.GetTaskAssigment(taskId);

                bool taskAssigned = await _homeChoreRepository.CheckOrHomeChoreWasAssigned(taskId);

                if (!taskAssigned)
                {
                    await _homeChoreRepository.DeleteNotAssignedTasks(taskId);
                    await _homeChoreRepository.Delete(taskId);
                    await _homeChoreRepository.Save();
                }
                else
                {
                    await _homeChoreRepository.DeleteAssignedTasks(taskId);
                    await _homeChoreRepository.Save();

                    TaskAssignment findLastAssignedTask = await _homeChoreRepository.GetLastAssigmentTask(taskId);

                    HomeChoreTask getTask = await _homeChoreRepository.Get(taskId);
                    getTask.EndDate = findLastAssignedTask.EndDate;
                    await _homeChoreRepository.Update(getTask);
                    await _homeChoreRepository.Save();
                }

                

                return Ok($"Home chore base with ID {taskId} deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting the article: {ex.Message}");
            }
        }

        [HttpGet("Chore/{id}")]
        [Authorize]
        public async Task<IActionResult> GetHomeChore(int id)
        {
            try
            {
                var homeChore = await _homeChoreRepository.Get(id);

                if (homeChore == null)
                {
                    return NotFound("No home chore found");
                }

                return Ok(homeChore);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching home chore bases: {ex.Message}");
            }
        }

        [HttpPost("ChoreIsDone/{id}")]
        [Authorize]
        public async Task<IActionResult> GetHomeChore(int id, bool isDone)
        {
            try
            {
                var homeChore = await _homeChoreRepository.GetTaskAssigment(id);
                var task = await _homeChoreRepository.Get(homeChore.TaskId);
                var user = await _userRepository.GetUserById((int)homeChore.HomeMemberId);

                var pointHistory = await _gamificationRepository.GetPointsHistoryByTaskId(id);

                homeChore.IsDone = isDone;
                if (isDone && pointHistory == null)
                {
                    homeChore.Points = task.Points;
                    Notification notification = new Notification
                    {
                        Title = $"Done '{task.Name}' and earned {task.Points} points",
                        IsRead = false,
                        Time = DateTime.Now,
                        UserId = (int)homeChore.HomeMemberId,
                        User = user,
                    };

                    await _notificationRepository.CreateNotification(notification);

                    PointsHistory pointsHistory = new PointsHistory
                    {
                        EarnedPoints = task.Points,
                        HomeMemberId = (int)homeChore.HomeMemberId,
                        TaskId = (int)id,
                        HomeId = homeChore.HomeId,
                        Text = $"Done '{task.Name}'",
                        EarnedDate = DateTime.Now,
                    };

                    await _gamificationRepository.AddPointsHistory(pointsHistory);
                }
                else
                {
                    Notification notification = new Notification
                    {
                        Title = $"Removed '{task.Name}' earned {task.Points} points",
                        IsRead = false,
                        Time = DateTime.Now,
                        UserId = (int)homeChore.HomeMemberId,
                        User = user,
                    };

                    await _notificationRepository.CreateNotification(notification);
                    await _gamificationRepository.Delete(pointHistory.Id);
                    homeChore.Points = 0;
                }
                
                await _homeChoreRepository.UpdateTaskAssignment(homeChore);
                await _homeChoreRepository.Save();

                if (homeChore == null)
                {
                    return NotFound("No home chore found");
                }

                return Ok(homeChore);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching home chore bases: {ex.Message}");
            }
        }

        private int GetMinutesFromTimeLong(TimeLong timeLong)
        {
            switch (timeLong)
            {
                case TimeLong.fiveMinutes: return 5;
                case TimeLong.tenMinutes: return 10;
                case TimeLong.fifteenMinutes: return 15;
                case TimeLong.twentyMinutes: return 20;
                case TimeLong.thirtyMinutes: return 30;
                case TimeLong.fourtyFiveMinutes: return 45;
                case TimeLong.hour: return 60;
                case TimeLong.hourAndHalf: return 90;
                case TimeLong.twoHours: return 120;
                case TimeLong.twoHoursAndHalf: return 150;
                case TimeLong.threeHours: return 180;
                case TimeLong.threeHoursAndHalf: return 210;
                case TimeLong.fourHours: return 240;
                case TimeLong.fourHoursAndHalf: return 270;
                case TimeLong.fiveHours: return 300;
                default: return 0;
            }
        }

        private async Task SetHomeChoreDates(HomeChoreTask homeChore)
        {
            try
            {
                DateTime startDate = (DateTime)homeChore.StartDate;
                DateTime endDate = (DateTime)homeChore.EndDate;
                int id = homeChore.Id;
                
                TaskSchedule taskSchedule = new TaskSchedule
                {
                    TaskId = id,
                    StartDate = startDate,
                    EndDate = endDate,
                };

                await _homeChoreRepository.SetHomeChoreDates(taskSchedule);
                await _homeChoreRepository.Save();


                if (homeChore.Unit == RepeatUnit.Day)
                {
                    int numberOfDays = (int)(endDate - startDate).TotalDays;

                    for (int i = 0; i <= numberOfDays; i += homeChore.Interval)
                    {
                        DateTime assignmentDate = startDate.AddDays(i);

                        TaskAssignment taskAssignment = new TaskAssignment
                        {
                            TaskId = id,
                            StartDate = assignmentDate,
                            EndDate = assignmentDate.AddMinutes(GetMinutesFromTimeLong(homeChore.Time)),
                            IsDone = false,
                            IsApproved = false,
                            HomeId = homeChore.HomeId
                        };

                        await _homeChoreRepository.AddTaskAssignment(taskAssignment);
                        await _homeChoreRepository.Save();
                    }
                }
                if (homeChore.Unit == RepeatUnit.Week)
                {
                    int totalWeeks = (int)Math.Ceiling((endDate - startDate).TotalDays / 7.0);

                    foreach (var selectedDay in homeChore.DaysOfWeek)
                    {
                        for (int i = 0; i < totalWeeks; i += homeChore.Interval)
                        {
                            DateTime assignmentDate = startDate.AddDays(i * 7);

                            // Find the next occurrence of the selected day of the week
                            int daysToAdd = ((int)selectedDay - (int)assignmentDate.DayOfWeek + 7) % 7;
                            assignmentDate = assignmentDate.AddDays(daysToAdd);

                            TaskAssignment taskAssignment = new TaskAssignment
                            {
                                TaskId = id,
                                StartDate = assignmentDate,
                                EndDate = assignmentDate.AddMinutes(GetMinutesFromTimeLong(homeChore.Time)),
                                IsDone = false,
                                IsApproved = false,
                                HomeId = homeChore.HomeId
                            };

                            await _homeChoreRepository.AddTaskAssignment(taskAssignment);
                        }
                    }

                    await _homeChoreRepository.Save();
                }

                if (homeChore.Unit == RepeatUnit.Month)
                {
                    int totalMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;

                    for (int i = 0; i < totalMonths; i += homeChore.Interval)
                    {
                        DateTime assignmentDate = startDate.AddMonths(i);

                        if (homeChore.MonthlyRepeatType == MonthlyRepeatType.DayOfMonth)
                        {
                            if (homeChore.DayOfMonth.HasValue)
                            {
                                int maxDayOfMonth = DateTime.DaysInMonth(assignmentDate.Year, assignmentDate.Month);
                                int selectedDayOfMonth = homeChore.DayOfMonth.Value;
                                selectedDayOfMonth = Math.Min(selectedDayOfMonth, maxDayOfMonth);

                                assignmentDate = new DateTime(assignmentDate.Year, assignmentDate.Month, selectedDayOfMonth);
                            }
                            else
                            {
                                assignmentDate = new DateTime(assignmentDate.Year, assignmentDate.Month, 1);
                            }
                        }
                        else if (homeChore.MonthlyRepeatType == MonthlyRepeatType.FirstDayOfWeek)
                        {
                            assignmentDate = new DateTime(assignmentDate.Year, assignmentDate.Month, 1);
                        }

                        TaskAssignment taskAssignment = new TaskAssignment
                        {
                            TaskId = id,
                            StartDate = assignmentDate,
                            EndDate = assignmentDate.AddMinutes(GetMinutesFromTimeLong(homeChore.Time)),
                            IsDone = false,
                            IsApproved = false,
                            HomeId = homeChore.HomeId
                        };

                        await _homeChoreRepository.AddTaskAssignment(taskAssignment);
                        await _homeChoreRepository.Save();
                    }
                }
                if (homeChore.Unit == RepeatUnit.Year)
                {
                    int totalYears = endDate.Year - startDate.Year;

                    for (int i = 0; i <= totalYears; i += homeChore.Interval)
                    {
                        DateTime assignmentDate = startDate.AddYears(i);

                        if ((i + 1) % homeChore.Interval == 0)
                        {
                            TaskAssignment taskAssignment = new TaskAssignment
                            {
                                TaskId = id,
                                StartDate = assignmentDate,
                                EndDate = assignmentDate.AddMinutes(GetMinutesFromTimeLong(homeChore.Time)),
                                IsDone = false,
                                IsApproved = false,
                                HomeId = homeChore.HomeId
                            };

                            await _homeChoreRepository.AddTaskAssignment(taskAssignment);
                            await _homeChoreRepository.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in SetHomeChoreDates: {ex}");
                throw; // Re-throw the exception to maintain the error propagation
            }
        }

        [HttpGet("Chore/Calendar/{id}")]
        [Authorize]
        public async Task<IActionResult> GetHomeChoresToCalendar(int id)
        {
            var homeChores = await _homeChoreRepository.GetCalendar(id);
            if (homeChores == null)
            {
                return NotFound($"Home chore base with ID {id} not found");
            }

            List<TaskAssignmentResponse> taskAssignments = new List<TaskAssignmentResponse>();

            foreach (var taskAssignment in homeChores)
            {
                HomeChoreTask homeChoreTask = await _homeChoreRepository.Get(taskAssignment.TaskId);

                TaskAssignmentResponse assignmentResponse = new TaskAssignmentResponse
                {
                    Id = taskAssignment.Id,
                    StartDate = taskAssignment.StartDate,
                    EndDate = taskAssignment.EndDate,
                    Task = homeChoreTask,
                    TaskId = taskAssignment.TaskId,
                    HomeMemberId = taskAssignment.HomeMemberId,
                    HomeId = taskAssignment.HomeId,
                    IsDone = taskAssignment.IsDone,
                    TotalVotes = await _homeChoreRepository.GetTotalVotes(taskAssignment.Id),
                    IsApproved = taskAssignment.IsApproved,
                };

                taskAssignments.Add(assignmentResponse);
            }

            return Ok(taskAssignments);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHomeChore(int id, [FromBody] HomeChoreBaseRequest homeChoreBaseRequest)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

                var homeChore = await _homeChoreRepository.Get(id);

                if (homeChore == null)
                {
                    return NotFound($"Home chore base with ID {id} not found");
                }

                TaskAssignment task = await _homeChoreRepository.GetTaskAssigment(homeChore.Id);

                if (homeChore.StartDate == null && homeChore.EndDate == null)
                {
                    List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();

                    if (homeChoreBaseRequest.DaysOfWeek == null)
                    {
                        dayOfWeeks.Add(DayOfWeek.Default);
                    }
                    else
                    {
                        foreach (int day in homeChoreBaseRequest.DaysOfWeek)
                        {
                            if (day == 0)
                            {
                                if (day == 0)
                                {
                                    dayOfWeeks.Add(DayOfWeek.Default);
                                }
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

                    homeChore.Id = id;
                    homeChore.Name = homeChoreBaseRequest.Name;
                    homeChore.ChoreType = homeChoreBaseRequest.ChoreType;
                    homeChore.Description = homeChoreBaseRequest.Description;
                    homeChore.Points = homeChoreBaseRequest.Points;
                    homeChore.LevelType = homeChoreBaseRequest.LevelType;
                    homeChore.Time = homeChoreBaseRequest.Time;
                    homeChore.Interval = homeChoreBaseRequest.Interval;
                    homeChore.Unit = homeChoreBaseRequest.Unit;
                    homeChore.DaysOfWeek = dayOfWeeks;
                    homeChore.DayOfMonth = homeChoreBaseRequest.DayOfMonth;
                    homeChore.MonthlyRepeatType = homeChoreBaseRequest.MonthlyRepeatType;
                    homeChore.StartDate = homeChoreBaseRequest.StartDate;
                    homeChore.EndDate = homeChoreBaseRequest.EndDate;

                    await _homeChoreRepository.Update(homeChore);
                    await _homeChoreRepository.Save();



                    await SetHomeChoreDates(homeChore);

                    await _homeChoreRepository.Save();
                    return Ok($"Home chore base with ID {id} updated successfully");
                }

                bool taskAssigned = await _homeChoreRepository.CheckOrHomeChoreWasAssigned(id);

                if (taskAssigned)
                {
                    HomeChoreRequest homeChoreRequest = new HomeChoreRequest();
                    homeChoreRequest.Name = homeChoreBaseRequest.Name;
                    homeChoreRequest.ChoreType = homeChoreBaseRequest.ChoreType;
                    homeChoreRequest.Description = homeChoreBaseRequest.Description;
                    homeChoreRequest.Points = homeChoreBaseRequest.Points;
                    homeChoreRequest.LevelType = homeChoreBaseRequest.LevelType;
                    homeChoreRequest.Time = homeChoreBaseRequest.Time;
                    homeChoreRequest.Interval = homeChoreBaseRequest.Interval;
                    homeChoreRequest.Unit = homeChoreBaseRequest.Unit;
                    homeChoreRequest.DaysOfWeek = homeChoreBaseRequest.DaysOfWeek;
                    homeChoreRequest.DayOfMonth = homeChoreBaseRequest.DayOfMonth;
                    homeChoreRequest.MonthlyRepeatType = homeChoreBaseRequest.MonthlyRepeatType;
                    homeChoreRequest.StartDate = homeChoreBaseRequest.StartDate;
                    homeChoreRequest.EndDate = homeChoreBaseRequest.EndDate;
                    homeChoreRequest.HomeId = homeChore.HomeId;

                    HomeChoreTask newTask = await _homeChoreRepository.CreateHomeChore(homeChoreRequest, userId, null);
                    await _homeChoreRepository.Save();


                    await SetHomeChoreDates(newTask);
                    await _homeChoreRepository.DeleteAssignedTasks(homeChore.Id);
                    await _homeChoreRepository.Save();

                    return Ok($"Home chore base with ID {id} updated successfully");
                }
                else
                {
                    List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();

                    if (homeChoreBaseRequest.DaysOfWeek == null)
                    {
                        dayOfWeeks.Add(DayOfWeek.Default);
                    }
                    else
                    {
                        foreach (int day in homeChoreBaseRequest.DaysOfWeek)
                        {
                            if (day == 0)
                            {
                                if (day == 0)
                                {
                                    dayOfWeeks.Add(DayOfWeek.Default);
                                }
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

                    homeChore.Id = id;
                    homeChore.Name = homeChoreBaseRequest.Name;
                    homeChore.ChoreType = homeChoreBaseRequest.ChoreType;
                    homeChore.Description = homeChoreBaseRequest.Description;
                    homeChore.Points = homeChoreBaseRequest.Points;
                    homeChore.LevelType = homeChoreBaseRequest.LevelType;
                    homeChore.Time = homeChoreBaseRequest.Time;
                    homeChore.Interval = homeChoreBaseRequest.Interval;
                    homeChore.Unit = homeChoreBaseRequest.Unit;
                    homeChore.DaysOfWeek = dayOfWeeks;
                    homeChore.DayOfMonth = homeChoreBaseRequest.DayOfMonth;
                    homeChore.MonthlyRepeatType = homeChoreBaseRequest.MonthlyRepeatType;
                    homeChore.StartDate = homeChoreBaseRequest.StartDate;
                    homeChore.EndDate = homeChoreBaseRequest.EndDate;

                    await _homeChoreRepository.Update(homeChore);
                    await _homeChoreRepository.Save();


                    await _homeChoreRepository.DeleteNotAssignedTasks(homeChore.Id);
                    await _homeChoreRepository.Save();

                    await SetHomeChoreDates(homeChore);

                    await _homeChoreRepository.Save();
                    return Ok($"Home chore base with ID {id} updated successfully");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the home chore base: {ex.Message}");
            }
        }

        [HttpPut("VoteTask/{taskId}/{voteValue}")]
        [Authorize]
        public async Task<IActionResult> VoteArticle(int taskId, int voteValue)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                var voteTask = await _homeChoreRepository.VoteArtical(taskId, userId, voteValue);

                TaskAssignment assignment = await _homeChoreRepository.GetTaskAssigment(taskId);
                HomeChoreTask task = await _homeChoreRepository.Get(assignment.TaskId);

                List<UserGetResponse> homeMembers = await _homeRepository.GetHomeMembers(assignment.HomeId);

                int votes = await _homeChoreRepository.GetTotalVotes(taskId);
                int halfHomes = homeMembers.Count / 2;

                if (votes >= halfHomes)
                {
                    assignment.Points = -10;
                    await _homeChoreRepository.UpdateTaskAssignment(assignment);
                    await _homeChoreRepository.Save();
                }
                else
                {
                    assignment.Points = task.Points;
                    await _homeChoreRepository.UpdateTaskAssignment(assignment);
                    await _homeChoreRepository.Save();
                }


                if (voteTask)
                {
                    return Ok(voteTask);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
