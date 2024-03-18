using AutoMapper;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using DayOfWeek = HomeChoreTracker.Api.Constants.DayOfWeek;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeChoreController : Controller
    {
        private readonly IHomeChoreRepository _homeChoreRepository;
        private readonly IHomeChoreBaseRepository _homeChoreBaseRepository;
        private readonly IMapper _mapper;

        public HomeChoreController(IHomeChoreRepository homeChoreRepository, IMapper mapper, IHomeChoreBaseRepository homeChoreBaseRepository)
        {
            _homeChoreRepository = homeChoreRepository;
            _mapper = mapper;
            _homeChoreBaseRepository = homeChoreBaseRepository;
        }

        [HttpPost("{homeId}")]
        [Authorize]
        public async Task<IActionResult> AddHomeChore(int homeId, int taskId)
        {
            try
            {
                var homeChoreBase = await _homeChoreBaseRepository.GetChoreBase(taskId);

                await _homeChoreRepository.AddHomeChoreBase(homeChoreBase, homeId);

                return Ok($"Home chore was added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding the home chore: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateHomeChoreBase(HomeChoreRequest homeChoreBaseRequest)
        {
            await _homeChoreRepository.CreateHomeChore(homeChoreBaseRequest);
            await _homeChoreRepository.Save();
            return Ok("Home chore base created successfully");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetHomeChoresBase(int id)
        {
            try
            {
                var homeChore = await _homeChoreRepository.GetAll(id);

                if (homeChore == null || !homeChore.Any())
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

        [HttpDelete("{taskId}")]
        [Authorize]
        public async Task<IActionResult> DeleteHomeChore(int taskId)
        {
            try
            {
                await _homeChoreRepository.Delete(taskId);
                await _homeChoreRepository.Save();

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

        [HttpPost("Chore/Dates/{id}")]
        [Authorize]
        public async Task <IActionResult> SetHomeChoreDates(int id, [FromBody] SetHomeChoreDatesRequest homeChoreDatesRequest)
        {
            var homeChore = await _homeChoreRepository.Get(id);
            if (homeChore == null)
            {
                return NotFound($"Home chore base with ID {id} not found");
            }

            TaskSchedule taskSchedule = new TaskSchedule
            {
                TaskId = id,
                StartDate = homeChoreDatesRequest.StartDate,
                EndDate = homeChoreDatesRequest.EndDate,
            };

            await _homeChoreRepository.SetHomeChoreDates(taskSchedule);
            await _homeChoreRepository.Save();


            if (homeChore.Unit == RepeatUnit.Day)
            {
                int numberOfDays = (int)(homeChoreDatesRequest.EndDate - homeChoreDatesRequest.StartDate).TotalDays;

                for (int i = 0; i <= numberOfDays; i += homeChore.Interval)
                {
                    DateTime assignmentDate = homeChoreDatesRequest.StartDate.AddDays(i);

                    TaskAssignment taskAssignment = new TaskAssignment
                    {
                        TaskId = id,
                        StartDate = assignmentDate,
                        EndDate = assignmentDate,
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
                int totalWeeks = (int)Math.Ceiling((homeChoreDatesRequest.EndDate - homeChoreDatesRequest.StartDate).TotalDays / 7.0);

                for (int i = 0; i < totalWeeks; i += homeChore.Interval)
                {
                    DateTime assignmentDate = homeChoreDatesRequest.StartDate.AddDays(i * 7);

                    if (homeChore.DaysOfWeek == null || homeChore.DaysOfWeek.Contains(DayOfWeek.Default))
                    {
                        Random rnd = new Random();
                        int randomDay = rnd.Next(1, 8);
                        assignmentDate = assignmentDate.AddDays(randomDay - (int)assignmentDate.DayOfWeek);
                    }
                    else
                    {
                        DayOfWeek firstDayOfWeek = homeChore.DaysOfWeek.First();
                        int daysToAdd = ((int)firstDayOfWeek - (int)assignmentDate.DayOfWeek + 7) % 7;
                        assignmentDate = assignmentDate.AddDays(daysToAdd);
                    }

                    TaskAssignment taskAssignment = new TaskAssignment
                    {
                        TaskId = id,
                        StartDate = assignmentDate,
                        EndDate = assignmentDate,
                        IsDone = false,
                        IsApproved = false,
                        HomeId = homeChore.HomeId
                    };

                    await _homeChoreRepository.AddTaskAssignment(taskAssignment);
                    await _homeChoreRepository.Save();
                }
            }
            if (homeChore.Unit == RepeatUnit.Month)
            {
                int totalMonths = ((homeChoreDatesRequest.EndDate.Year - homeChoreDatesRequest.StartDate.Year) * 12) + homeChoreDatesRequest.EndDate.Month - homeChoreDatesRequest.StartDate.Month;

                for (int i = 0; i < totalMonths; i += homeChore.Interval)
                {
                    DateTime assignmentDate = homeChoreDatesRequest.StartDate.AddMonths(i);

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
                        EndDate = assignmentDate,
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
                int totalYears = homeChoreDatesRequest.EndDate.Year - homeChoreDatesRequest.StartDate.Year;

                for (int i = 0; i <= totalYears; i += homeChore.Interval)
                {
                    DateTime assignmentDate = homeChoreDatesRequest.StartDate.AddYears(i);

                    if ((i + 1) % homeChore.Interval == 0)
                    {
                        TaskAssignment taskAssignment = new TaskAssignment
                        {
                            TaskId = id,
                            StartDate = assignmentDate,
                            EndDate = assignmentDate,
                            IsDone = false,
                            IsApproved = false,
                            HomeId = homeChore.HomeId
                        };

                        await _homeChoreRepository.AddTaskAssignment(taskAssignment);
                        await _homeChoreRepository.Save();
                    }
                }
            }

            return Ok("Home chore date created successfully");
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

            foreach(var taskAssignment in homeChores)
            {
                HomeChoreTask homeChoreTask = await _homeChoreRepository.Get(taskAssignment.TaskId);

                TaskAssignmentResponse assignmentResponse = new TaskAssignmentResponse
                {
                    Id = taskAssignment.Id,
                    StartDate = taskAssignment.StartDate,
                    EndDate = taskAssignment.EndDate,
                    Title = homeChoreTask.Name,
                    TaskId = taskAssignment.TaskId,
                    HomeMemberId = taskAssignment.HomeMemberId,
                    HomeId = taskAssignment.HomeId,
                    IsDone = taskAssignment.IsDone,
                    IsApproved = taskAssignment.IsApproved,
                };

                taskAssignments.Add(assignmentResponse);
            }

            return Ok(taskAssignments);
        }

        [HttpGet("Chore/Dates/{id}")]
        [Authorize]
        public async Task<IActionResult>GetHomeChoreDates(int id)
        {
            var homeChore = await _homeChoreRepository.Get(id);
            if (homeChore == null)
            {
                return NotFound($"Home chore base with ID {id} not found");
            }

            var homeChores = await _homeChoreRepository.GetTaskSchedule(id);
            return Ok(homeChores);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHomeChore(int id, [FromBody] HomeChoreBaseRequest homeChoreBaseRequest)
        {
            try
            {
                var homeChore = await _homeChoreRepository.Get(id);

                if (homeChore == null)
                {
                    return NotFound($"Home chore base with ID {id} not found");
                }

                if (homeChore.WasEarnedPoints)
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
                    homeChoreRequest.HomeId = homeChore.HomeId;

                    await _homeChoreRepository.CreateHomeChore(homeChoreRequest);
                    await _homeChoreRepository.Save();

                    return Ok($"Home chore base with ID {id} updated successfully");
                }
                if (homeChore.IsActive)
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

                    await _homeChoreRepository.Update(homeChore);
                    await _homeChoreRepository.Save();

                    return Ok($"Home chore base with ID {id} updated successfully");
                }
                else
                {
                    return BadRequest($"An error occurred while updating the home chore");
                }


            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating the home chore base: {ex.Message}");
            }
        }
    }
}
