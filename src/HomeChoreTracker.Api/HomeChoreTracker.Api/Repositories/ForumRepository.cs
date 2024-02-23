using DocumentFormat.OpenXml.Wordprocessing;
using HomeChoreTracker.Api.Contracts.Forum;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
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
			List<UserHomes> userHomes = await _dbContext.UserHomes
				.Where(x => x.UserId == userId)
				.ToListAsync();

			List<int> otherUserIds = userHomes
				.Select(uh => uh.UserId)
				.Distinct()
				.ToList();

			var advices = await _dbContext.Advices
				.Where(e => e.UserId == userId || otherUserIds.Contains(e.UserId) || e.IsPublic)
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
				UserName = advice.UserId == userId ? _dbContext.Users.Where(x => x.Id == userId).FirstOrDefault()?.UserName : _dbContext.Users.Where(x => x.Id == advice.UserId).FirstOrDefault()?.UserName
			}).ToList();

			return adviceResponses;
		}

		public async Task<Advice> GetAdviceById(int id)
		{
			return await _dbContext.Advices.FindAsync(id);
		}

		public async Task UpdateAdvice(Advice advice)
		{
			_dbContext.Advices.Update(advice);
			await _dbContext.SaveChangesAsync();
		}
	}
}
