using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.HomeChore;
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

namespace HomeChoreTracker.Api.Tests.Repositories
{
	[Collection("Sequential")]
	public class HomeChoreRepositoryTests
	{
		private readonly HomeChoreTrackerDbFixture _homeChoreTrackerDbFixture;

		public HomeChoreRepositoryTests(HomeChoreTrackerDbFixture trakiDbFixture)
		{
			_homeChoreTrackerDbFixture = trakiDbFixture;
		}
		[Fact]
		public async Task AddHomeChoreBase_ShouldAddHomeChoreBase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);
			var homeChoreBase = new HomeChoreBase
			{
				Name = "Test Chore",
				Description = "Test Description",
				Points = 10,
				Time = Constants.TimeLong.hour,
				LevelType = LevelType.Hard,
				Interval = 1,
				Unit = RepeatUnit.Week,
				DaysOfWeek = new List<Constants.DayOfWeek> { Constants.DayOfWeek.Monday, Constants.DayOfWeek.Tuesday, Constants.DayOfWeek.Wednesday }, 
				DayOfMonth = 15,
				MonthlyRepeatType = MonthlyRepeatType.FirstDayOfWeek,
				ChoreType = HomeChoreType.Bedroom
			};
			var homeId = 1;

			// Act
			var task = await repository.AddHomeChoreBase(homeChoreBase, homeId);
			var addedChoreBase = await repository.Get(task.Id);

			// Assert
			Assert.NotNull(addedChoreBase);
			Assert.Equal(task.Name, addedChoreBase.Name);
			Assert.Equal(task.Description, addedChoreBase.Description);
			Assert.Equal(task.Points, addedChoreBase.Points);
		}

		[Fact]
		public async Task AddTaskAssignment_ShouldAddTaskAssignment()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);
			var taskAssignment = new TaskAssignment
			{
			};

			// Act
			await repository.AddTaskAssignment(taskAssignment);
			var addedAssignment = await context.TaskAssignments.FindAsync(taskAssignment.Id);

			// Assert
			Assert.NotNull(addedAssignment);
		}

		[Fact]
		public async Task CreateHomeChore_ShouldCreateHomeChore()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);
			var homeChoreRequest = new HomeChoreRequest
			{
			};
			var userId = 1;
			var homeId = 1;

			// Act
			var result = await repository.CreateHomeChore(homeChoreRequest, userId, homeId);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task RemoveTaskAssignment_ShouldRemoveTaskAssignment()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);
			var taskAssignment = new TaskAssignment
			{
			};
			await context.TaskAssignments.AddAsync(taskAssignment);
			await context.SaveChangesAsync();

			// Act
			await repository.RemoveTaskAssignment(taskAssignment.Id);
			var removedAssignment = await context.TaskAssignments.FindAsync(taskAssignment.Id);

			// Assert
			Assert.Null(removedAssignment);
		}

		[Fact]
		public async Task VoteArtical_ShouldVoteForArticle()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);
			var taskId = 1;
			var userId = 1;
			var voteValue = 1;

			// Act
			var result = await repository.VoteArtical(taskId, userId, voteValue);

			// Assert
			Assert.True(result);
		}
	}
}
