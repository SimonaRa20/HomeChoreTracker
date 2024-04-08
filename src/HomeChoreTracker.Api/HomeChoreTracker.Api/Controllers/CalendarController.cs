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
using NodaTime;
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

    [HttpGet("Chore/{id}")]
    [Authorize]
    public async Task<IActionResult> GetHomeChoreFromCalendar(int id)
    {
        TaskAssignment task = await _homeChoreRepository.GetTaskAssigment(id);

        HomeChoreTask homeChoreTask = await _homeChoreRepository.Get(task.TaskId);

        HomeChoreEventResponse homeChoreEventResponse = new HomeChoreEventResponse
        {
            Id = task.Id,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            Name = homeChoreTask.Name,
            ChoreType = homeChoreTask.ChoreType.ToString(),
            Description = homeChoreTask.Description?.ToString(),
            Points = homeChoreTask.Points,
            LevelType = homeChoreTask.LevelType.ToString(),
            Time = homeChoreTask.Time.ToString(),
            IsDone = task.IsDone
        };

        return Ok(homeChoreEventResponse);
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
                List<BusyInterval> busyIntervals = await _userRepository.GetUserBusyIntervals(minPointsMember.Key);

                HomeChoreTask hometask = await _homeChoreRepository.Get(task.TaskId);
                List<(DateTime start, DateTime end)> suitableIntervals = FindHomersSuitableTimeIntervals(user, task.StartDate, user.CalendarEvents, hometask.Time, userTasksAssigned, busyIntervals);

                if (suitableIntervals.Any())
                {
                    var (start, end) = suitableIntervals.First();
                    task.StartDate = start;
                    task.EndDate = start.AddMinutes(GetMinutesFromTimeLong(hometask.Time));
                }
                else
                {
                    await _homeChoreRepository.RemoveTaskAssignment(task.Id);
                    continue;
                }

                await _homeChoreRepository.UpdateTaskAssignment(task);

                assignedPoints[minPointsMember.Key] += hometask.Points;
            }

            return Ok("Tasks assigned successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    private List<(DateTime start, DateTime end)> FindHomersSuitableTimeIntervals(User user, DateTime startTime, List<Event> events, TimeLong choreTime, List<TaskAssignment> assignedTasks, List<BusyInterval> busyIntervals)
    {
        List<(DateTime start, DateTime end)> suitableIntervals = new List<(DateTime start, DateTime end)>();
        DateTime currentDate = startTime.Date;
        TimeSpan startDayTime = user.StartDayTime;
        TimeSpan endDayTime = user.EndDayTime;

        while (true)
        {
            suitableIntervals = GetSuitableIntervalsForDay(user, currentDate, startDayTime, endDayTime, events, choreTime, assignedTasks, busyIntervals);

            if (suitableIntervals.Any())
            {
                break;
            }

            currentDate = currentDate.AddDays(1);

            if (currentDate - startTime.Date > TimeSpan.FromDays(7))
            {
                suitableIntervals = new List<(DateTime, DateTime)>();
                break;
            }
        }

        return suitableIntervals;
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> AssignTaskToMember(int id, AssignedHomeMember assignedHomeMember)
    {
        try
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            User user = await _userRepository.GetUserById(userId);

            TaskAssignment taskAssignment = await _homeChoreRepository.GetTaskAssigment(assignedHomeMember.TaskId);
            if (assignedHomeMember.HomeMemberId == null)
            {
                taskAssignment.HomeMemberId = null;
                await _homeChoreRepository.UpdateTaskAssignment(taskAssignment);
                await _homeChoreRepository.Save();
                return Ok();
            }

            taskAssignment.HomeMemberId = assignedHomeMember.HomeMemberId;

            HomeChoreTask task = await _homeChoreRepository.Get(taskAssignment.TaskId);

            User assignedUser = await _userRepository.GetUserById(assignedHomeMember.HomeMemberId.Value);
            List<TaskAssignment> userTasksAssigned = await _homeChoreRepository.GetAssignedTasks(assignedUser.Id);
            List<BusyInterval> busyIntervals = await _userRepository.GetUserBusyIntervals(assignedUser.Id);

            List<(DateTime start, DateTime end)> suitableIntervals = FindSuitableTimeIntervals(user, taskAssignment.StartDate, user.CalendarEvents, task.Time, userTasksAssigned, busyIntervals);

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
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    private List<(DateTime start, DateTime end)> FindSuitableTimeIntervals(User user, DateTime startTime, List<Event> events, TimeLong choreTime, List<TaskAssignment> assignedTasks, List<BusyInterval> busyIntervals)
    {
        List<(DateTime start, DateTime end)> suitableIntervals = new List<(DateTime start, DateTime end)>();

        DateTime currentDate = startTime.Date;
        TimeSpan startDayTime = user.StartDayTime;
        TimeSpan endDayTime = user.EndDayTime;

        while (true)
        {
            suitableIntervals = GetSuitableIntervalsForDay(user, currentDate, startDayTime, endDayTime, events, choreTime, assignedTasks, busyIntervals);

            if (suitableIntervals.Any())
            {
                break;
            }

            currentDate = currentDate.AddDays(1);
        }

        return suitableIntervals;
    }

    private List<(DateTime start, DateTime end)> GetSuitableIntervalsForDay(User user, DateTime date, TimeSpan startDayTime, TimeSpan endDayTime, List<Event> events, TimeLong choreTime, List<TaskAssignment> assignedTasks, List<BusyInterval> busyIntervals)
    {
        List<(DateTime start, DateTime end)> suitableIntervals = new List<(DateTime start, DateTime end)>();

        DateTime dayStart = date.Date.Add(startDayTime);
        DateTime dayEnd = date.Date.Add(endDayTime);

        suitableIntervals.Add((dayStart, dayEnd));

        if (events != null)
        {
            foreach (var e in events)
            {
                List<(DateTime start, DateTime end)> updatedIntervals = new List<(DateTime start, DateTime end)>();

                foreach (var interval in suitableIntervals)
                {
                    if (interval.end > e.StartDate && interval.start < e.EndDate)
                    {
                        if (interval.start < e.StartDate)
                            updatedIntervals.Add((interval.start, e.StartDate));
                        if (interval.end > e.EndDate)
                            updatedIntervals.Add((e.EndDate, interval.end));
                    }
                    else
                    {
                        updatedIntervals.Add(interval);
                    }
                }

                suitableIntervals = updatedIntervals;
            }
        }
        busyIntervals = busyIntervals.OrderBy(x => x.StartTime).ToList();

        foreach (BusyInterval busyInterval in busyIntervals)
        {
            foreach (var interval in suitableIntervals.ToList())
            {
                if (interval.end.TimeOfDay > busyInterval.StartTime && interval.start.TimeOfDay < busyInterval.EndTime)
                {
                    suitableIntervals.Remove(interval);
                    if (interval.start.TimeOfDay < busyInterval.StartTime)
                        suitableIntervals.Add((interval.start, interval.start.Add(busyInterval.StartTime - interval.start.TimeOfDay)));
                    if (interval.end.TimeOfDay > busyInterval.EndTime)
                        suitableIntervals.Add((interval.start.Add(busyInterval.EndTime - interval.start.TimeOfDay), interval.end));
                }
            }
        }

        suitableIntervals = suitableIntervals
            .Where(i => (i.end - i.start).TotalMinutes >= GetMinutesFromTimeLong(choreTime))
            .ToList();

        foreach (var assignedTask in assignedTasks)
        {
            foreach (var interval in suitableIntervals.ToList())
            {
                if (interval.end.TimeOfDay > assignedTask.StartDate.TimeOfDay && interval.start.TimeOfDay < assignedTask.EndDate.TimeOfDay)
                {
                    suitableIntervals.Remove(interval);
                    if (interval.start.TimeOfDay < assignedTask.StartDate.TimeOfDay)
                        suitableIntervals.Add((interval.start, interval.start.Add(assignedTask.StartDate.TimeOfDay - interval.start.TimeOfDay)));
                    if (interval.end.TimeOfDay > assignedTask.StartDate.TimeOfDay)
                        suitableIntervals.Add((interval.start.Add(assignedTask.EndDate.TimeOfDay - interval.start.TimeOfDay), interval.end));
                }
            }
            suitableIntervals.RemoveAll(interval =>
            assignedTask.StartDate < interval.end &&
            assignedTask.EndDate > interval.start);
        }
        return suitableIntervals;
    }


    private IEnumerable<(DateTime start, DateTime end)> GetFreeIntervals(DateTime referenceDate, DateTime eventStart, DateTime eventEnd, int startHour, int endHour)
    {
        if (eventStart.Date == referenceDate.Date)
        {
            if (eventStart.Hour > startHour)
                yield return (referenceDate.Date.AddHours(startHour), eventStart);

            if (eventEnd.Hour < endHour)
                yield return (eventEnd, referenceDate.Date.AddHours(endHour));
        }
        else
        {
            yield return (referenceDate.Date.AddHours(startHour), referenceDate.Date.AddHours(endHour));
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
