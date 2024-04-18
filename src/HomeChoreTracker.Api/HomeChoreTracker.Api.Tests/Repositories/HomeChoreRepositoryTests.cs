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
		public async Task CreateHomeChore_DaysOfWeekNotNull()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);
			var homeChoreRequest = new HomeChoreRequest
			{
				DaysOfWeek = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 },
				IsPublic = true,
													  
			};
			var userId = 1;
			var homeId = 1;

			// Act
			var result = await repository.CreateHomeChore(homeChoreRequest, userId, homeId);

			// Assert
			Assert.NotNull(result);
			Assert.NotNull(result.DaysOfWeek);
			Assert.NotEmpty(result.DaysOfWeek);
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

		[Fact]
		public async Task Delete_RemovesHomeChoreTask()
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

			var task = await repository.AddHomeChoreBase(homeChoreBase, homeId);
			var addedChoreBase = await repository.Get(task.Id);

			// Act
			await repository.Delete(addedChoreBase.Id);
			await repository.Save();

			// Assert
			var deletedHomeChore = await context.HomeChoreTasks.FindAsync(addedChoreBase.Id);
			Assert.Null(deletedHomeChore);
		}

		[Fact]
		public async Task CheckOrHomeChoreWasAssigned_ReturnsTrue_WhenHomeChoreWasAssigned()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);

			var taskAssignment = new TaskAssignment
			{
				TaskId = 1,
				HomeMemberId = 1
			};

			await context.TaskAssignments.AddAsync(taskAssignment);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.CheckOrHomeChoreWasAssigned(taskAssignment.TaskId);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task CheckOrHomeChoreWasAssigned_ReturnsFalse_WhenHomeChoreWasNotAssigned()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);

			var taskAssignment = new TaskAssignment
			{
				TaskId = 1,
				HomeMemberId = null
			};

			await context.TaskAssignments.AddAsync(taskAssignment);
			await context.SaveChangesAsync();

			// Act
			var result = await repository.CheckOrHomeChoreWasAssigned(taskAssignment.TaskId);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task GetUnassignedTasks_ReturnsUnassignedTasks()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);

			var homeId = 1;

			// Act
			var result = await repository.GetUnassignedTasks(homeId);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task DeleteAssignedTasks_RemovesAssignedTasks()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);

			var taskId = 1;

			var taskAssignment = new TaskAssignment
			{
				TaskId = taskId,
				HomeMemberId = 1
			};
			await context.TaskAssignments.AddAsync(taskAssignment);
			await context.SaveChangesAsync();

			// Act
			await repository.DeleteAssignedTasks(taskId);

			// Assert
			var deletedTaskAssignment = await context.TaskAssignments.FindAsync(taskAssignment.Id);
			Assert.Null(deletedTaskAssignment);
		}

		[Fact]
		public async Task DeleteNotAssignedTasks_RemovesNotAssignedTasks()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);

			var taskId = 1;

			var taskAssignment = new TaskAssignment
			{
				TaskId = taskId,
				HomeMemberId = null
			};
			await context.TaskAssignments.AddAsync(taskAssignment);
			await context.SaveChangesAsync();

			// Act
			await repository.DeleteNotAssignedTasks(taskId);
			await repository.Save();

			// Assert
			var deletedTaskAssignment = await context.TaskAssignments.FindAsync(taskAssignment.Id);
			Assert.Null(deletedTaskAssignment);
		}

		[Fact]
		public async Task Update_ShouldUpdateHomeChoreTask()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);

			var homeChoreTask = new HomeChoreBase
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

			// Act
			var task = await repository.AddHomeChoreBase(homeChoreTask,1);
			task.Name = "Updated Name";
			await repository.Update(task);
			var updatedTask = await repository.Get(task.Id);

			// Assert
			Assert.NotNull(updatedTask);
			Assert.Equal("Updated Name", updatedTask.Name);
		}

		[Fact]
		public async Task UpdateTaskAssignment_ShouldUpdateTaskAssignment()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);

			var taskAssignment = new TaskAssignment
			{
			};

			// Act
			await repository.AddTaskAssignment(taskAssignment);
			await repository.Save();
			taskAssignment.IsDone = true;
			await repository.UpdateTaskAssignment(taskAssignment);
			var updatedAssignment = await repository.GetTaskAssigment(taskAssignment.Id);

			// Assert
			Assert.NotNull(updatedAssignment);
			Assert.Equal(true, updatedAssignment.IsDone);
		}

		[Fact]
		public async Task GetTotalPointsAssigned_ShouldReturnTotalPoints()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new HomeChoreRepository(context);

			var memberId = 1;

			var taskAssignment1 = new TaskAssignment
			{
				HomeMemberId = memberId,
				TaskId = 1,
				IsDone = false
			};
			var taskAssignment2 = new TaskAssignment
			{
				HomeMemberId = memberId,
				TaskId = 2,
				IsDone = true
			};

			await context.TaskAssignments.AddRangeAsync(taskAssignment1, taskAssignment2);
			await context.SaveChangesAsync();

			// Act
			var totalPoints = await repository.GetTotalPointsAssigned(memberId);

			// Assert
			Assert.Equal(20, totalPoints);
		}
	}
}
