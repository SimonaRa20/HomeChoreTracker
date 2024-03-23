using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;

        public CalendarRepository(HomeChoreTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddEvent(Event calendarEvent)
        {
            await _dbContext.Events.AddAsync(calendarEvent);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Event>> GetAll(int userId)
        {
            return await _dbContext.Events.Where(x => x.UserId.Equals(userId)).ToListAsync();
        }


    }
}
