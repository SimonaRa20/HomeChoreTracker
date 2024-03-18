using HomeChoreTracker.Api.Contracts.Calendar;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CalendarController : Controller
    {
        private readonly ICalendarRepository _calendarRepository;

        public CalendarController(ICalendarRepository calendarRepository)
        {
            _calendarRepository = calendarRepository;
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
    }
}
