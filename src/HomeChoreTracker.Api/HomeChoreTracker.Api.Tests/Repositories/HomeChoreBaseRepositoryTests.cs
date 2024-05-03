using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using HomeChoreTracker.Api.Tests.DatabaseFixture;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DayOfWeek = HomeChoreTracker.Api.Constants.DayOfWeek;

namespace HomeChoreTracker.Api.Tests.Repositories
{
	[Collection("Sequential")]
	public class HomeChoreBaseRepositoryTests
	{
		private readonly HomeChoreTrackerDbFixture _homeChoreTrackerDbFixture;

		public HomeChoreBaseRepositoryTests(HomeChoreTrackerDbFixture trakiDbFixture)
		{
			_homeChoreTrackerDbFixture = trakiDbFixture;
		}

		[Fact]
		public async Task AddHomeChoreBase_ShouldAddHomeChoreBase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreBaseRepository(context);
			var homeChoreBase = new HomeChoreBaseRequest
			{
				Name = "Test Chore",
				Description = "Test Description",
				Points = 10,
				HoursTime = 1,
				MinutesTime = 30,
				LevelType = Constants.LevelType.Medium,
				Interval = 1,
				Unit = Constants.RepeatUnit.Week,
				DaysOfWeek = new List<int> { 1, 2, 3, 4, 5, 6, 7 },
				DayOfMonth = 15,
				MonthlyRepeatType = null,
				ChoreType = HomeChoreType.Organize
			};

			// Act
			await repository.AddHomeChoreBase(homeChoreBase);
			var addedChoreBase = await context.HomeChoresBases.FirstOrDefaultAsync();

			// Assert
			Assert.NotNull(addedChoreBase);
		}

		[Fact]
		public async Task UpdateHomeChoreBase_ShouldUpdateHomeChoreBase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreBaseRepository(context);
			var homeChoreBase = new HomeChoreBaseRequest
			{
				Name = "Test Chore",
				Description = "Test Description",
				Points = 10,
				HoursTime = 1,
				MinutesTime = 30,
				LevelType = Constants.LevelType.Medium,
				Interval = 1,
				Unit = Constants.RepeatUnit.Week,
				DaysOfWeek = null,
				DayOfMonth = 15,
				MonthlyRepeatType = null,
				ChoreType = HomeChoreType.Organize
			};
			var task = await repository.AddHomeChoreBase(homeChoreBase);

			task.Name = "New title";

			// Act
			await repository.Update(task);
			await repository.Save();
			var updatedChoreBase = await context.HomeChoresBases.FindAsync(task.Id);

			// Assert
			Assert.NotNull(updatedChoreBase);
			Assert.Equal("New title", updatedChoreBase.Name);
			Assert.Equal(homeChoreBase.Description, updatedChoreBase.Description);
			Assert.Equal(homeChoreBase.Points, updatedChoreBase.Points);
		}

		[Fact]
		public async Task DeleteHomeChoreBase_ShouldDeleteHomeChoreBase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreBaseRepository(context);
			var homeChoreBase = new HomeChoreBaseRequest
			{
				Name = "Test Chore",
				Description = "Test Description",
				Points = 10,
				HoursTime = 1,
				MinutesTime = 30,
				LevelType = Constants.LevelType.Medium,
				Interval = 1,
				Unit = Constants.RepeatUnit.Week,
				DaysOfWeek = new List<int> { 0 },
				DayOfMonth = 15,
				MonthlyRepeatType = null,
				ChoreType = HomeChoreType.Organize
			};

			// Act
			var task = await repository.AddHomeChoreBase(homeChoreBase);

			// Act
			await repository.Delete(task.Id);
			var deletedChoreBase = await context.HomeChoresBases.FindAsync(task.Id);

			// Assert
			Assert.Null(deletedChoreBase);
		}
	}
}
