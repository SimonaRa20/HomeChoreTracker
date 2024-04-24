using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Ical.Net.CalendarComponents;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
	public class AvatarRepository : IAvatarRepository
	{
		private readonly HomeChoreTrackerDbContext _dbContext;

		public AvatarRepository(HomeChoreTrackerDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAvatar(Avatar avatar)
		{
			await _dbContext.Avatars.AddAsync(avatar);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<List<Avatar>> GetAll()
		{
			return await _dbContext.Avatars.ToListAsync();
		}

		public async Task Update(Avatar avatar)
		{
			_dbContext.Entry(avatar).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
		}

		public async Task<Avatar> GetAvatar(int id)
		{
			return await _dbContext.Avatars.Where(x=>x.Id.Equals(id)).FirstOrDefaultAsync();
		}
	}
}
