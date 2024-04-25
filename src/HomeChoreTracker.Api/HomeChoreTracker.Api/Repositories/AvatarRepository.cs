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

		public async Task<List<AvatarPurchase>> GetPurchaseAvatars(int userId)
		{
			return await _dbContext.AvatarPurchases.Where(x=>x.UserId.Equals(userId)).ToListAsync();
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

        public async Task<Avatar> GetUserAvatar(int userId)
        {
			var avatarId = await _dbContext.Users.Where(x=>x.Id.Equals(userId)).Select(x=>x.AvatarId).FirstOrDefaultAsync();
            return await _dbContext.Avatars.Where(x => x.Id.Equals(avatarId)).FirstOrDefaultAsync();
        }

        public async Task<List<PointsHistory>> GetPointsHistoryByUserId(int userId)
		{
			return await _dbContext.PointsHistory.Where(x => x.HomeMemberId.Equals(userId)).ToListAsync();
		}

		public async Task SetAvatar(int userId, Avatar avatar)
		{
			var user = await _dbContext.Users.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
			user.AvatarId = avatar.Id;
			user.Avatar = avatar;
            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

		public async Task CountThePoints(int userId, Avatar avatar)
		{
			var user = await _dbContext.Users.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
			int points = 0;
			if(avatar.AvatarType.Equals(AvatarType.Intermediate))
			{
				points = 100;
			}
			else if(avatar.AvatarType.Equals(AvatarType.Advanced))
			{
				points = 1000;
			}
			else
			{
				points = 10000;
			}

			PointsHistory pointsHistory = new PointsHistory
			{
				EarnedPoints = -points,
				Text = $"Purchased avatar. -{points}",
				EarnedDate = DateTime.Now,
				HomeMemberId = userId,
			};

			await _dbContext.PointsHistory.AddAsync(pointsHistory);
			await _dbContext.SaveChangesAsync();

			Notification notification = new Notification
			{
				Title = $"Purchased avatar. -{points}",
				IsRead = false,
				Time = DateTime.Now,
				UserId = userId,
				User = user
			};

			await _dbContext.Notifications.AddAsync(notification);
			await _dbContext.SaveChangesAsync();

			AvatarPurchase avatarPurchase = new AvatarPurchase
			{
				UserId = userId,
				AvatarId = avatar.Id,
				User = user,
				Avatar = avatar
			};

			await _dbContext.AvatarPurchases.AddAsync(avatarPurchase);
			await _dbContext.SaveChangesAsync();
		}
	}
}
