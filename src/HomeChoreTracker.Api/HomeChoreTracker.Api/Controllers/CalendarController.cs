using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Calendar;
using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

[Route("[controller]")]
[ApiController]
public class CalendarController : Controller
{
    private readonly ICalendarRepository _calendarRepository;
    private readonly IHomeChoreRepository _homeChoreRepository;
    private readonly IUserRepository _userRepository;

    public CalendarController(ICalendarRepository calendarRepository, IHomeChoreRepository homeChoreRepository, IUserRepository userRepository)
    {
        _calendarRepository = calendarRepository;
        _homeChoreRepository = homeChoreRepository;
        _userRepository = userRepository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddEventsFromFile([FromForm] CalendarRequest calendarRequest)
    {
        try
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (calendarRequest.File == null || calendarRequest.File.Length == 0)
                return BadRequest("File is empty");

            using (var streamReader = new StreamReader(calendarRequest.File.OpenReadStream()))
            {
                var content = await streamReader.ReadToEndAsync();

                var calendar = Calendar.Load(content);

                foreach (var component in calendar.Children)
                {
                    if (component is CalendarEvent calendarEvent)
                    {
                        var newEvent = new Event
                        {
                            UserId = userId,
                            StartDate = calendarEvent.Start.AsSystemLocal,
                            EndDate = calendarEvent.End.AsSystemLocal,
                            Summary = calendarEvent.Summary
                        };

                        await _calendarRepository.AddEvent(newEvent);
                    }
                }

                return Ok("Events added successfully");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetEvents()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

        List<Event> events = await _calendarRepository.GetAll(userId);
        return Ok(events);
    }

    [HttpGet("Chores")]
    [Authorize]
    public async Task<IActionResult> GetHomeChoresToCalendar()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

        var homeChores = await _homeChoreRepository.GetHomeChoresUserCalendar(userId);
        if (homeChores == null)
        {
            return NotFound($"Home chore base with user ID {userId} not found");
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
                IsApproved = taskAssignment.IsApproved,
            };

            taskAssignments.Add(assignmentResponse);
        }

        return Ok(taskAssignments);
    }


    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> AssignTaskToMember(int id, AssignedHomeMember assignedHomeMember)
    {
        try
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            User user = await _userRepository.GetUserById(userId);

            if(assignedHomeMember.HomeMemberId == null)
            {
                TaskAssignment taskAs = await _homeChoreRepository.GetTaskAssigment(assignedHomeMember.TaskId);
                taskAs.HomeMemberId = null;
                await _homeChoreRepository.UpdateTaskAssignment(taskAs);
                await _homeChoreRepository.Save();
                return Ok();
            }


            TaskAssignment taskAssignment = await _homeChoreRepository.GetTaskAssigment(assignedHomeMember.TaskId);
            taskAssignment.HomeMemberId = assignedHomeMember.HomeMemberId;

            HomeChoreTask task = await _homeChoreRepository.Get(taskAssignment.TaskId);

            List<TaskAssignment> userTasksAssigned = new List<TaskAssignment>();

            if (assignedHomeMember.HomeMemberId != null)
            {
                userTasksAssigned = await _homeChoreRepository.GetAssignedTasks((int)assignedHomeMember.HomeMemberId);
            }

            List<(DateTime start, DateTime end)> suitableIntervals = FindSuitableTimeIntervals(user, taskAssignment.StartDate, user.CalendarEvents, task.Time, userTasksAssigned);

            if (suitableIntervals.Any())
            {
                var (start, end) = suitableIntervals.First();
                taskAssignment.StartDate = start;
                taskAssignment.EndDate = start.AddMinutes(GetMinutesFromTimeLong(task.Time));
            }
            else
            {
                return BadRequest("No suitable time intervals found for the task.");
            }

            await _homeChoreRepository.UpdateTaskAssignment(taskAssignment);
            await _homeChoreRepository.Save();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpPost("AssignTasksToMembers/{homeId}")]
    [Authorize]
    public async Task<IActionResult> AssignTasksToMembers(int homeId)
    {
        try
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            List<User> members = await _userRepository.GetHomeMembers(homeId);

            List<TaskAssignment> unassignedTasks = await _homeChoreRepository.GetUnassignedTasks(homeId);

            if (unassignedTasks == null || unassignedTasks.Count == 0)
            {
                return BadRequest("There are no unassigned tasks available.");
            }

            Dictionary<int, int> assignedPoints = new Dictionary<int, int>();
            foreach (var member in members)
            {
                int totalPoints = await _homeChoreRepository.GetTotalPointsAssigned(member.Id);
                assignedPoints.Add(member.Id, totalPoints);
            }

            foreach (var task in unassignedTasks)
            {
                var minPointsMember = assignedPoints.OrderBy(x => x.Value).First();

                task.HomeMemberId = minPointsMember.Key;

                User user = await _userRepository.GetUserById(minPointsMember.Key);

                List<TaskAssignment> userTasksAssigned = await _homeChoreRepository.GetAssignedTasks(minPointsMember.Key);

                HomeChoreTask hometask = await _homeChoreRepository.Get(task.TaskId);
                List<(DateTime start, DateTime end)> suitableIntervals = FindSuitableTimeIntervals(user, task.StartDate, user.CalendarEvents, hometask.Time, userTasksAssigned);

                if (suitableIntervals.Any())
                {
                    var (start, end) = suitableIntervals.First();
                    task.StartDate = start;
                    task.EndDate = start.AddMinutes(GetMinutesFromTimeLong(hometask.Time));
                }
                else
                {
                    return BadRequest("No suitable time intervals found for the task.");
                }

                await _homeChoreRepository.UpdateTaskAssignment(task);
                await _homeChoreRepository.Save();
                
                assignedPoints[minPointsMember.Key] += hometask.Points;
            }

            return Ok("Tasks assigned successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }


    private List<(DateTime start, DateTime end)> FindSuitableTimeIntervals(User user, DateTime StartTime, List<Event> events, TimeLong choreTime, List<TaskAssignment> assignedTasks)
    {
        List<(DateTime start, DateTime end)> suitableIntervals = new List<(DateTime start, DateTime end)>();

        TimeSpan lunchStart = TimeSpan.FromHours(user.StartLunchHour);
        TimeSpan lunchEnd = TimeSpan.FromHours(user.EndLunchHour);
        TimeSpan nightEnd = TimeSpan.FromHours(user.EndDayHour);

        if (events == null || events.Count == 0)
        {
            // If there are no events, assume the user has a free day
            DateTime dayStart = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, user.StartDayHour, user.StartDayMinutes, 0);
            DateTime dayEnd = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, user.EndDayHour, user.EndDayMinutes, 0);
            suitableIntervals.Add((dayStart, dayEnd));
        }
        else
        {
            events = events.Where(e => e.StartDate > StartTime).OrderBy(e => e.StartDate).ToList();

            List<(DateTime start, DateTime end)> freeIntervals = GetFreeIntervals(events, StartTime, lunchStart, lunchEnd, nightEnd);

            foreach (var interval in freeIntervals)
            {
                if (IsIntervalSuitable(interval.start, interval.end, user))
                {
                    suitableIntervals.Add(interval);
                }
            }
        }

        suitableIntervals = suitableIntervals
            .Where(i => (i.end - i.start).TotalMinutes >= GetMinutesFromTimeLong(choreTime))
            .Where(i => i.start.TimeOfDay >= TimeSpan.FromHours(user.StartDayHour))
            .ToList();

        foreach (var assignedTask in assignedTasks)
        {
            suitableIntervals = suitableIntervals.SelectMany(interval =>
            {
                // If the assigned task overlaps with the interval, adjust the interval accordingly
                if (assignedTask.StartDate < interval.end && assignedTask.EndDate > interval.start)
                {
                    if (assignedTask.StartDate <= interval.start && assignedTask.EndDate >= interval.end)
                    {
                        // Task fully encompasses the interval, remove it
                        return Enumerable.Empty<(DateTime start, DateTime end)>();
                    }
                    else if (assignedTask.StartDate <= interval.start)
                    {
                        // Task overlaps the start of the interval
                        return new[]
                        {
                        (assignedTask.EndDate, interval.end)
                    };
                    }
                    else if (assignedTask.EndDate >= interval.end)
                    {
                        // Task overlaps the end of the interval
                        return new[]
                        {
                        (interval.start, assignedTask.StartDate)
                    };
                    }
                    else
                    {
                        // Task is in the middle of the interval
                        return new[]
                        {
                        (interval.start, assignedTask.StartDate),
                        (assignedTask.EndDate, interval.end)
                    };
                    }
                }
                // If there's no overlap, keep the interval as is
                return new[] { interval };
            }).ToList();
        }

        return suitableIntervals;
    }


    private List<(DateTime start, DateTime end)> GetFreeIntervals(List<Event> events, DateTime startTime, TimeSpan lunchStart, TimeSpan lunchEnd, TimeSpan nightEnd)
    {
        List<(DateTime start, DateTime end)> freeIntervals = new List<(DateTime start, DateTime end)>();

        DateTime currentTime = startTime;
        while (currentTime < startTime.AddDays(1).Date)
        {
            DateTime nextTime = currentTime.AddHours(1);

            if (!(currentTime.TimeOfDay >= lunchStart && currentTime.TimeOfDay < lunchEnd) && currentTime.TimeOfDay < nightEnd)
            {
                freeIntervals.Add((currentTime, nextTime));
            }

            currentTime = nextTime;
        }

        foreach (var e in events)
        {
            freeIntervals.RemoveAll(i => (i.start >= e.StartDate && i.start < e.EndDate) || (i.end > e.StartDate && i.end <= e.EndDate));
        }

        return freeIntervals;
    }

    private bool IsIntervalSuitable(DateTime start, DateTime end, User user)
    {
        if (user.Morning && start.TimeOfDay < TimeSpan.FromHours(12))
        {
            return true;
        }
        if (user.MiddleDay && start.TimeOfDay >= TimeSpan.FromHours(12) && start.TimeOfDay < TimeSpan.FromHours(18))
        {
            return true;
        }
        if (user.Evening && end.TimeOfDay >= TimeSpan.FromHours(18))
        {
            return true;
        }
        return false;
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

    [HttpGet("Chores/File")]
    [Authorize]
    public async Task<IActionResult> HomeChoreCalendarFile()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

        var homeChores = await _homeChoreRepository.GetHomeChoresUserCalendar(userId);
        if (homeChores == null)
        {
            return NotFound($"Home chore base with user ID {userId} not found");
        }

        var calendar = new Calendar();

        foreach (var taskAssignment in homeChores)
        {
            HomeChoreTask homeChoreTask = await _homeChoreRepository.Get(taskAssignment.TaskId);

            var evt = new CalendarEvent
            {
                Start = new CalDateTime(taskAssignment.StartDate),
                End = new CalDateTime(taskAssignment.EndDate),
                Summary = homeChoreTask.Name,
            };

            calendar.Events.Add(evt);
        }

        var serializer = new CalendarSerializer();
        var serializedCalendar = serializer.SerializeToString(calendar);

        var bytes = Encoding.UTF8.GetBytes(serializedCalendar);
        var contentType = "text/calendar";
        var fileName = "HomeChores.ics";

        return File(bytes, contentType, fileName);
    }

}
