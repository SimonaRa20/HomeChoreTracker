using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using HomeChoreTracker.Api.Tests.DatabaseFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.Repositories
{
	[Collection("Sequential")]
	public class UserRepositoryTests
	{
		private readonly HomeChoreTrackerDbFixture _homeChoreTrackerDbFixture;

		public UserRepositoryTests(HomeChoreTrackerDbFixture trakiDbFixture)
		{
			_homeChoreTrackerDbFixture = trakiDbFixture;
		}

		[Fact]
		public async Task AddBusyInterval_ShouldAddBusyIntervalToDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new UserRepository(context);
			var busyInterval = new BusyInterval
			{
				Day = DayOfWeek.Monday,
				StartTime = new TimeSpan(12,00,00),
				EndTime = new TimeSpan(12,30,00),
				UserId = 1
			};

			// Act
			await repository.AddBusyInterval(busyInterval);

			// Assert
			var addedAdvice = await repository.GetBusyIntervalById(busyInterval.Id);
			Assert.NotNull(addedAdvice);
		}

		[Fact]
		public async Task DeleteBusyInterval_ShouldDeleteBusyIntervalToDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new UserRepository(context);
			var busyInterval = new BusyInterval
			{
				Day = DayOfWeek.Monday,
				StartTime = new TimeSpan(12, 00, 00),
				EndTime = new TimeSpan(12, 30, 00),
				UserId = 1
			};
			await repository.AddBusyInterval(busyInterval);
			// Act
			await repository.DeleteInterval(busyInterval.Id);

			// Assert
			var deletedBusyInterval = await repository.GetBusyIntervalById(busyInterval.Id);
			Assert.Null(deletedBusyInterval);
		}

		[Fact]
		public async Task UpdateBusyInterval_ShouldUpdateBusyIntervalToDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new UserRepository(context);
			
			var busyInterval = new BusyInterval
			{
				Day = DayOfWeek.Monday,
				StartTime = new TimeSpan(12, 00, 00),
				EndTime = new TimeSpan(12, 30, 00),
				UserId = 1
			};
			await repository.AddBusyInterval(busyInterval);
			var updateInterval = busyInterval;
			updateInterval.StartTime = new TimeSpan(11, 00, 00);

			// Act
			await repository.UpdateInterval(updateInterval);

			// Assert
			var updatedInterval = await repository.GetBusyIntervalById(updateInterval.Id);
			Assert.NotEqual(new TimeSpan(12, 00, 00), updatedInterval.StartTime);
		}

		[Fact]
		public async Task GetUserIdByEmail_ShouldReturnUserId()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var authRepository = new AuthRepository(context);
			var repository = new UserRepository(context);
			User user = new User
			{
				UserName = "user1",
				Email = "user@gmail.com",
				Password = "kk",
				Role = "Admin",
				StartDayTime = new TimeSpan(8, 0, 0),
				EndDayTime = new TimeSpan(22, 0, 0)
			};

			await authRepository.AddUser(user);
			await authRepository.Save();
			var userEmail = "user@gmail.com";
			var expectedUserId = 2;

			// Act
			var userId = await repository.GetUserIdByEmail(userEmail);

			// Assert
			Assert.Equal(expectedUserId, userId);
		}

		[Fact]
		public async Task GetHomeMembers_ShouldReturnListOfUsers()
		{
			// Arrange
			var homeId = 1;
			var expectedMemberCount = 1;
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var authRepository = new AuthRepository(context);
			var repository = new UserRepository(context);
			var gamificationRepository = new GamificationRepository(context, repository);
			var homeRepository = new HomeRepository(context, repository, gamificationRepository);

			GamificationLevel gamificationLevel = new GamificationLevel
			{
				LevelId = 1,
			};

			await gamificationRepository.AddLevel(gamificationLevel);

			User user = new User
			{
				UserName = "user1",
				Email = "user@gmail.com",
				Password = "kk",
				Role = "Admin",
				StartDayTime = new TimeSpan(8, 0, 0),
				EndDayTime = new TimeSpan(22, 0, 0)
			};

			await authRepository.AddUser(user);
			await authRepository.Save();

			HomeRequest home = new HomeRequest
			{
				Title = "Test",
			};

			await homeRepository.CreateHome(home, user.Id);

			// Act
			var members = await repository.GetHomeMembers(homeId);

			// Assert
			Assert.NotNull(members);
			Assert.Equal(expectedMemberCount, members.Count);
		}

		[Fact]
		public async Task UpdateUser_ShouldUpdateUserDetails()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var authRepository = new AuthRepository(context);
			User user = new User
			{
				UserName = "user1",
				Email = "user@gmail.com",
				Password = "kk",
				Role = "Admin",
				StartDayTime = new TimeSpan(8, 0, 0),
				EndDayTime = new TimeSpan(22, 0, 0)
			};

			await authRepository.AddUser(user);
			await authRepository.Save();
			
			var updatedUserName = "UpdatedUserName";
			
			var repository = new UserRepository(context);
			var findUser = await repository.GetUserById(user.Id);
			user.UserName = updatedUserName;

			// Act
			await repository.UpdateUser(user);

			// Assert
			var updatedUser = await repository.GetUserById(user.Id);
			Assert.Equal(updatedUserName, updatedUser.UserName);
		}

	}
}
