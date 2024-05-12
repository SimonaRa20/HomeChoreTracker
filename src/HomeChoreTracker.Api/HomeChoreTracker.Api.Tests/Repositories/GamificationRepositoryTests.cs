using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using HomeChoreTracker.Api.Tests.DatabaseFixture;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.Repositories
{
	[Collection("Sequential")]
	public class GamificationRepositoryTests
	{
		private readonly HomeChoreTrackerDbFixture _homeChoreTrackerDbFixture;

		public GamificationRepositoryTests(HomeChoreTrackerDbFixture trakiDbFixture)
		{
			_homeChoreTrackerDbFixture = trakiDbFixture;
		}

		[Fact]
		public async Task CreateFirstLevel_And_GetIt()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			var newLevel = new GamificationLevel
			{
				LevelId = 1,
			};

			// Act
			await repository.AddLevel(newLevel);

			// Assert
			var retrievedLevel = await repository.GetGamificationLevelById(newLevel.Id);
			Assert.NotNull(retrievedLevel);
			Assert.Equal(newLevel.LevelId, retrievedLevel.LevelId);
		}

		[Fact]
		public async Task Update_GamificationLevel()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			var newLevel = new GamificationLevel
			{
				LevelId = 1,
				PointsRequired = 100
			};
			await repository.AddLevel(newLevel);

			newLevel.PointsRequired = 50;

			// Act
			await repository.Update(newLevel);

			// Assert
			var updatedLevel = await context.GamificationLevels.FindAsync(newLevel.Id);
			Assert.Equal(50, updatedLevel.PointsRequired);
		}

		[Fact]
		public async Task GetPointsHistoryByTaskId_ReturnsCorrectPointsHistory()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int taskId = 1;
			var pointsHistory = new PointsHistory
			{
				TaskId = taskId,
				Text = "Text"
			};
			await repository.AddPointsHistory(pointsHistory);

			// Act
			var retrievedPointsHistory = await repository.GetPointsHistoryByTaskId(taskId);

			// Assert
			Assert.NotNull(retrievedPointsHistory);
		}

		[Fact]
		public async Task AddPointsHistory_AddsNewPointsHistory()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			var pointsHistory = new PointsHistory
			{
				Text = "Text",
			};

			// Act
			await repository.AddPointsHistory(pointsHistory);

			// Assert
			var addedPointsHistory = await context.PointsHistory.FindAsync(pointsHistory.Id);
			Assert.NotNull(addedPointsHistory);
		}

		[Fact]
		public async Task Delete_RemovesPointsHistory()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			var pointsHistory = new PointsHistory
			{
				Text = "Text",
			};
			await repository.AddPointsHistory(pointsHistory);

			// Act
			await repository.Delete(pointsHistory.Id);

			// Assert
			var deletedPointsHistory = await context.PointsHistory.FindAsync(pointsHistory.Id);
			Assert.Null(deletedPointsHistory);
		}

		[Fact]
		public async Task GetHomeThisWeekPointsHistory_ReturnsCorrectPointsHistory()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int homeId = 1;

			DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
			DateTime endOfWeek = startOfWeek.AddDays(6);
			var pointsHistory = new List<PointsHistory>
			{
				new PointsHistory { TaskId = homeId, Text = "Text", EarnedDate = startOfWeek.AddDays(1), EarnedPoints = 10 },
				new PointsHistory { TaskId = homeId, Text = "Text", EarnedDate = startOfWeek.AddDays(2), EarnedPoints = 20 },
            };
			await context.PointsHistory.AddRangeAsync(pointsHistory);
			await context.SaveChangesAsync();

			// Act
			var retrievedPointsHistory = await repository.GetHomeThisWeekPointsHistory(homeId);

			// Assert
			Assert.NotNull(retrievedPointsHistory);
			Assert.Equal(0, retrievedPointsHistory.Count);
		}

		[Fact]
		public async Task GetHomePreviousWeekPointsHistory_ReturnsCorrectPointsHistory()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int homeId = 1;

			DateTime startOfPreviousWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1).AddDays(-7);
			DateTime endOfPreviousWeek = startOfPreviousWeek.AddDays(6);
			var pointsHistory = new List<PointsHistory>
			{
				new PointsHistory { TaskId = homeId, Text = "Text", EarnedDate = startOfPreviousWeek.AddDays(1), EarnedPoints = 15 },
				new PointsHistory { TaskId = homeId, Text = "Text", EarnedDate = startOfPreviousWeek.AddDays(2), EarnedPoints = 25 },
            };
			await context.PointsHistory.AddRangeAsync(pointsHistory);
			await context.SaveChangesAsync();

			// Act
			var retrievedPointsHistory = await repository.GetHomePreviousWeekPointsHistory(homeId);

			// Assert
			Assert.NotNull(retrievedPointsHistory);
			Assert.Equal(0, retrievedPointsHistory.Count);
		}

		[Fact]
		public async Task UpdateBadgeWallet_UpdatesWallet()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			var wallet = new BadgeWallet
			{
				DoneFirstTask = true,
				UserId = 1
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			wallet.DoneFirstTask = false;

			// Act
			await repository.UpdateBadgeWallet(wallet);

			// Assert
			var updatedWallet = await context.BadgeWallets.FindAsync(wallet.Id);
			Assert.False(updatedWallet.DoneFirstTask);
		}

		[Theory]
		[InlineData(false)]
		public async Task UserHasCreateFirstIncomeBadge_ReturnsCorrectValue(bool expected)
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				CreateFirstIncome = expected,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasCreateFirstIncomeBadge(userId);

			// Assert
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData(false)]
		public async Task UserHasDoneFirstTaskBadge_ReturnsCorrectValue(bool expected)
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				DoneFirstTask = expected,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasDoneFirstTaskBadge(userId);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public async Task UserHasCreateFirstExpenseBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				CreateFirstExpense = true,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasCreateFirstExpenseBadge(userId);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task UserHasCreateFirstAdviceBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				CreateFirstAdvice = false,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasCreateFirstAdviceBadge(userId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UserHasCreateFirstPurchaseBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				CreateFirstPurchase = false,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasCreateFirstPurchaseBadge(userId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UserHasCreatedTaskWasUsedOtherHomeBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				CreatedTaskWasUsedOtherHome = false,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasCreatedTaskWasUsedOtherHomeBadge(userId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UserHasDoneFirstCleaningTaskBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				DoneFirstCleaningTask = false,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasDoneFirstCleaningTaskBadge(userId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UserHasDoneFirstLaundryTaskBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				DoneFirstLaundryTask = false,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasDoneFirstLaundryTaskBadge(userId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UserHasDoneFirstKitchenTaskBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				DoneFirstKitchenTask = false,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasDoneFirstKitchenTaskBadge(userId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UserHasDoneFirstBathroomTaskBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				DoneFirstBathroomTask = false,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasDoneFirstBathroomTaskBadge(userId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UserHasDoneFirstBedroomTaskBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				DoneFirstBedroomTask = false,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasDoneFirstBedroomTaskBadge(userId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UserHasDoneFirstOutdoorsTaskBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				DoneFirstOutdoorsTask = false,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasDoneFirstOutdoorsTaskBadge(userId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UserHasDoneFirstOrganizeTaskBadge_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var wallet = new BadgeWallet
			{
				DoneFirstOrganizeTask = false,
				UserId = userId
			};
			await context.BadgeWallets.AddAsync(wallet);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasDoneFirstOrganizeTaskBadge(userId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UserHasEarnedHundredPointsPerDay_ReturnsCorrectValue()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepositoryMock = new Mock<IUserRepository>();
			var repository = new GamificationRepository(context, userRepositoryMock.Object);

			int userId = 1;
			var pointsHistory = new PointsHistory
			{
				HomeMemberId = userId,
				Text = "Test",
				EarnedDate = DateTime.Today,
				EarnedPoints = 100
			};
			await context.PointsHistory.AddAsync(pointsHistory);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.UserHasEarnedHundredPointsPerDay(userId);

			// Assert
			Assert.True(result);
		}
	}
}
