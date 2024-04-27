using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
	public class ChallengeRepository : IChallengeRepository
	{
		private readonly HomeChoreTrackerDbContext _dbContext;

		public ChallengeRepository(HomeChoreTrackerDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddChallenge(Challenge challenge)
		{
			await _dbContext.AddAsync(challenge);
			await _dbContext.SaveChangesAsync();
		}
		
		public async Task<List<User>> GetUsersOpponents(int userId)
		{
			return await _dbContext.Users.Where(x => !x.Id.Equals(userId) && x.Role.Equals("User")).ToListAsync();
		}

		public async Task<List<User>> GetUsersByHome(int homeId)
		{
			var home = await _dbContext.Homes.Where(x=>x.Id.Equals(homeId)).FirstOrDefaultAsync();
			var usershomes = await _dbContext.UserHomes.Where(x => x.HomeId.Equals(home.Id)).Select(x=>x.UserId).ToListAsync();
			var users = await _dbContext.Users.Where(x => usershomes.Contains(x.Id)).ToListAsync();

			return users;
		}

		public async Task<List<Home>> GetOpponentsHomes(int userId)
		{
			var userHomes = await _dbContext.UserHomes.Where(x => x.UserId == userId).Select(h => h.Home).ToListAsync();
			var opponentHomes = await _dbContext.UserHomes.Select(h => h.Home).Where(x => !userHomes.Contains(x)).ToListAsync();
			return opponentHomes;
		}

		public async Task<List<Home>> GetUserHomes(int userId)
		{
			return await _dbContext.UserHomes.Where(x => x.UserId == userId).Select(h => h.Home).ToListAsync();
		}

		public async Task<Home> GetHome(int homeId)
		{
			return await _dbContext.Homes.Where(x => x.Id == homeId).FirstOrDefaultAsync();
		}

		public async Task<User> GetUser(int userId)
		{
			return await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
		}

		public async Task<List<Challenge>> GetReceivedChallenges()
		{
			return await _dbContext.Challenges.Where(x => x.Action.Equals(ChallengeInvitationType.None)).ToListAsync();
		}

		public async Task<Challenge> GetChallengeById(int challengeId)
		{
			return await _dbContext.Challenges.Where(x => x.Id.Equals(challengeId)).FirstOrDefaultAsync();
		}

		public async Task Update(Challenge challenge)
		{
			_dbContext.Entry(challenge).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
		}

		public async Task<List<Challenge>> GetCurrentChallenges()
		{
			var challenges = await _dbContext.Challenges.Where(x => x.Action.Equals(ChallengeInvitationType.Accept)).ToListAsync();
			challenges = challenges.Where(x => x.ResultType.Equals(ChallengeResultType.None)).ToList();
			challenges = challenges.Where(x=>x.ChallengeCount > x.Count || x.ChallengeCount > x.OpponentCount).ToList();
			challenges = challenges.Where(x => x.EndTime > DateTime.Now).ToList();
			return challenges;
		}

		public async Task<List<Challenge>> GetHistoryChallenges()
		{
			return await _dbContext.Challenges.Where(x => !x.ResultType.Equals(ChallengeInvitationType.None) && !x.ResultType.Equals(ChallengeResultType.None)).ToListAsync();
		}

		public async Task UpdateChallenge(TaskAssignment assignment)
		{
			List<Challenge> challenges = await GetCurrentChallenges();
			if (challenges != null)
			{
				foreach (Challenge challenge in challenges)
				{
					var cha = await UpdateChallengeCount(assignment, challenge);
					_dbContext.Entry(cha).State = EntityState.Modified;
					await _dbContext.SaveChangesAsync();
				}
			}
		}

		public async Task<Challenge> UpdateChallengeCount(TaskAssignment assignment, Challenge challenge)
		{
			HomeChoreTask task = await _dbContext.HomeChoreTasks.Where(x => x.Id.Equals(assignment.TaskId)).FirstOrDefaultAsync();

			if (OpponentType.User.Equals(challenge.OpponentType))
			{
				if (challenge.UserId.Equals(assignment.HomeMemberId))
				{
					challenge = await UpdateChallengePoints(challenge, task, false);
				}
				else if (challenge.OpponentUserId.Equals(assignment.HomeMemberId))
				{
					challenge = await UpdateChallengePoints(challenge, task, true);
				}
			}
			else
			{
				var userHomes = await GetUserHomes((int)assignment.HomeMemberId);
				var isUserHome = userHomes.Select(x => x.Id).Contains((int)challenge.HomeId);
				if (isUserHome)
				{
					Home home = userHomes.Where(x => x.Id.Equals(challenge.HomeId)).FirstOrDefault();
					if (challenge.HomeId.Equals(home.Id))
					{
						challenge = await UpdateChallengePoints(challenge, task, false);
					}
					else if (challenge.OpponentHomeId.Equals(home.Id))
					{
						challenge = await UpdateChallengePoints(challenge, task, true);
					}
				}
			}
			return challenge;
		}

		public async Task<Challenge> UpdateChallengePoints(Challenge challenge, HomeChoreTask homeChoreTask, bool opponent)
		{
			var count = 0;

			if (challenge.ChallengeType.Equals(ChallengeType.EarnPoints) || challenge.ChallengeType.Equals(ChallengeType.TasksAllCategories))
			{
				count += homeChoreTask.Points;
			}
			else if (challenge.ChallengeType.Equals(ChallengeType.TasksCleaningCategory) && homeChoreTask.ChoreType.Equals(HomeChoreType.Cleaning))
			{
				count += homeChoreTask.Points;
			}
			else if (challenge.ChallengeType.Equals(ChallengeType.TasksLaundryCategory) && homeChoreTask.ChoreType.Equals(HomeChoreType.Laundry))
			{
				count += homeChoreTask.Points;
			}
			else if (challenge.ChallengeType.Equals(ChallengeType.TasksKitchenCategory) && homeChoreTask.ChoreType.Equals(HomeChoreType.Kitchen))
			{
				count += homeChoreTask.Points;
			}
			else if (challenge.ChallengeType.Equals(ChallengeType.TasksBathroomCategory) && homeChoreTask.ChoreType.Equals(HomeChoreType.Bedroom))
			{
				count += homeChoreTask.Points;
			}
			else if (challenge.ChallengeType.Equals(ChallengeType.TasksBedroomCategory) && homeChoreTask.ChoreType.Equals(HomeChoreType.Bedroom))
			{
				count += homeChoreTask.Points;
			}
			else if (challenge.ChallengeType.Equals(ChallengeType.TasksOutdoorsCategory) && homeChoreTask.ChoreType.Equals(HomeChoreType.Outdoors))
			{
				count += homeChoreTask.Points;
			}
			else if (challenge.ChallengeType.Equals(ChallengeType.TasksOrganizeCategory) && homeChoreTask.ChoreType.Equals(HomeChoreType.Organize))
			{
				count += homeChoreTask.Points;
			}
			else
			{
				count += homeChoreTask.Points;
			}

			if (opponent)
			{
				challenge.OpponentCount += count;
			}
			else
			{
				challenge.Count += count;
			}

			return challenge;
		}
	}
}
