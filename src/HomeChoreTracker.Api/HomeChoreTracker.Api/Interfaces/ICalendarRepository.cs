using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface ICalendarRepository
    {
        Task AddEvent(Event calendarEvent);
        Task<List<Event>> GetAll(int userId);
    }
}
