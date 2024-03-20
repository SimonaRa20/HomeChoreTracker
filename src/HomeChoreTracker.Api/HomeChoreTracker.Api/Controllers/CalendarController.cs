using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Calendar;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    public async Task<IActionResult> GetAdvice()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

        List<Event> events = await _calendarRepository.GetAll(userId);
        return Ok(events);
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
            taskAssignment.HomeMemberId = assignedHomeMember.HomeMemberId;

            HomeChoreTask task = await _homeChoreRepository.Get(taskAssignment.TaskId);


            List<(DateTime start, DateTime end)> suitableIntervals = FindSuitableTimeIntervals(user, taskAssignment.StartDate, user.CalendarEvents, task.Time);

            if (suitableIntervals.Any())
            {
                var (start, end) = suitableIntervals.First();
                taskAssignment.StartDate = start;
                taskAssignment.EndDate = end;
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

    private List<(DateTime start, DateTime end)> FindSuitableTimeIntervals(User user, DateTime StartTime, List<Event> events, TimeLong choreTime)
    {
        List<(DateTime start, DateTime end)> suitableIntervals = new List<(DateTime start, DateTime end)>();

        TimeSpan lunchStart = TimeSpan.FromHours(user.StartLunchHour);
        TimeSpan lunchEnd = TimeSpan.FromHours(user.EndLunchHour);
        TimeSpan nightEnd = TimeSpan.FromHours(user.EndDayHour);

        events = events.Where(e => e.StartDate > StartTime).OrderBy(e => e.StartDate).ToList();

        List<(DateTime start, DateTime end)> freeIntervals = GetFreeIntervals(events, StartTime, lunchStart, lunchEnd, nightEnd);

        foreach (var interval in freeIntervals)
        {
            if (IsIntervalSuitable(interval.start, interval.end, user))
            {
                suitableIntervals.Add(interval);
            }
        }

        suitableIntervals = suitableIntervals
            .Where(i => (i.end - i.start).TotalMinutes >= GetMinutesFromTimeLong(choreTime)) // Ensure duration is sufficient
            .Where(i => i.start.TimeOfDay >= TimeSpan.FromHours(user.StartDayHour)) // Ensure tasks start at 8:00 AM or later
            .ToList();

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
}
