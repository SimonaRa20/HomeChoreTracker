using DocumentFormat.OpenXml.Wordprocessing;
using HomeChoreTracker.Api.Contracts.Forum;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using iText.StyledXmlParser.Node;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
	public class ForumRepository : IForumRepository
	{
		private readonly HomeChoreTrackerDbContext _dbContext;

		public ForumRepository(HomeChoreTrackerDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAdvice(Advice advice)
		{
			await _dbContext.Advices.AddAsync(advice);
			await _dbContext.SaveChangesAsync();
		}

		public async Task Delete(int id)
		{
			Advice advice = await _dbContext.Advices.FindAsync(id);
			if (advice != null)
			{
				_dbContext.Advices.Remove(advice);
			}
			await _dbContext.SaveChangesAsync();
		}

		public async Task<bool> HasPermission(int id, int userId)
		{
			Advice advice = await _dbContext.Advices.FindAsync(id);
			if(advice.UserId.Equals(userId))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public async Task<List<AdviceResponse>> GetAll(int userId)
		{
			List<int> homesList = await _dbContext.UserHomes
				.Where(x => x.UserId == userId)
				.Select(x => x.HomeId)
				.ToListAsync();

			List<int> userList = await _dbContext.UserHomes
				.Where(x => homesList.Contains(x.HomeId))
				.Select(x => x.UserId)
				.Distinct()
				.ToListAsync();

			var advices = await _dbContext.Advices
				.Where(e => e.IsPublic || homesList.Contains(e.UserId) || userList.Contains(e.UserId))
				.ToListAsync();

			var adviceResponses = advices.Select(advice => new AdviceResponse
			{
				Id = advice.Id,
				Title = advice.Title,
				Time = advice.Time,
				Type = advice.Type,
				Description = advice.Description,
				IsPublic = advice.IsPublic,
				UserId = advice.UserId,
				UserName = _dbContext.Users
					.Where(x => x.Id == advice.UserId)
					.Select(x => x.UserName)
					.FirstOrDefault()
			}).ToList();

			return adviceResponses;
		}

		public async Task<Advice> GetAdviceById(int id)
		{
			return await _dbContext.Advices.FindAsync(id);
		}

		public async Task UpdateAdvice(Advice advice)
		{
			// Find the existing advice entity being tracked
			var existingAdvice = await _dbContext.Advices.FindAsync(advice.Id);

			existingAdvice.Title = advice.Title;
			existingAdvice.Time = advice.Time;
			existingAdvice.Type = advice.Type;
			existingAdvice.Description = advice.Description;
			existingAdvice.IsPublic = advice.IsPublic;
			existingAdvice.UserId = advice.UserId;

			// Save the changes
			await _dbContext.SaveChangesAsync();
		}

	}
}
