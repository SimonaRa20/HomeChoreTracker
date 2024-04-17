using HomeChoreTracker.Api.Constants;
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
	public class IncomeRepositoryTests
	{
		private readonly HomeChoreTrackerDbFixture _homeChoreTrackerDbFixture;

		public IncomeRepositoryTests(HomeChoreTrackerDbFixture trakiDbFixture)
		{
			_homeChoreTrackerDbFixture = trakiDbFixture;
		}

		[Fact]
		public async Task AddCategory_ShouldAddNewCategoryToDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);
			var category = new FinancialCategory
			{
				Name = "Test Category",
				Type = FinancialType.Income
			};

			// Act
			var addedCategory = await repository.AddCategory(category);

			// Assert
			Assert.NotNull(addedCategory);
			Assert.True(addedCategory.Id > 0);
		}

		[Fact]
		public async Task CheckCategory_ShouldReturnExistingCategory()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);
			var categoryName = "Test Category";

			// Act
			var category = await repository.CheckCategory(categoryName);

			// Assert
			Assert.NotNull(category);
			Assert.Equal(categoryName, category.Name);
		}

		[Fact]
		public async Task AddIncome_ShouldAddNewIncomeToDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);
			var income = new FinancialRecord
			{
				Title = "Test Income",
				Amount = 100,
				Description = "Test Description",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 1 // Assuming category ID 1 exists
			};

			// Act
			await repository.AddIncome(income);

			// Assert
			var addedIncome = await context.FinancialRecords.FirstOrDefaultAsync(i => i.Title == income.Title);
			Assert.NotNull(addedIncome);
		}

		[Fact]
		public async Task GetIncomeById_ShouldReturnIncomeById()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);

			FinancialCategory category = new FinancialCategory
			{
				Name = "test",
				Type = FinancialType.Income,
				UserId = 1
			};

			await repository.AddCategory(category);

			var income = new FinancialRecord
			{
				Title = "Test Income",
				Amount = 100,
				Description = "Test Description",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 1,
				UserId = 1
			};
			await repository.AddIncome(income);
			var userId = 1;
			var income1 = new FinancialRecord
			{
				Title = "Income 1",
				Amount = 200,
				Description = "Description 1",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 1,
				UserId = userId
			};
			await repository.AddIncome(income1);
			var income2 = new FinancialRecord
			{
				Title = "Income 2",
				Amount = 300,
				Description = "Description 2",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 2,
				UserId = userId
			};
			await repository.AddIncome(income2);

			// Act
			var retrievedIncome = await repository.GetIncomeById(income.Id);

			// Assert
			Assert.NotNull(retrievedIncome);
			Assert.Equal(income.Title, retrievedIncome.Title);
		}

		[Fact]
		public async Task GetAll_ShouldReturnAllIncomeRecords()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);
			var income = new FinancialRecord
			{
				Title = "Test Income",
				Amount = 100,
				Description = "Test Description",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 1,
				UserId = 1
			};

			var userId = 1;
			var income1 = new FinancialRecord
			{
				Title = "Income 1",
				Amount = 200,
				Description = "Description 1",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 1,
				UserId = userId
			};
			var income2 = new FinancialRecord
			{
				Title = "Income 2",
				Amount = 300,
				Description = "Description 2",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 2,
				UserId = userId
			};
			await context.FinancialRecords.AddRangeAsync(income1, income2);
			await context.SaveChangesAsync();

			// Act
			var allIncomes = await repository.GetAll(1);

			// Assert
			Assert.NotNull(allIncomes);
			Assert.NotEmpty(allIncomes);
		}

		[Fact]
		public async Task GetCurrentMonthTotalIncome_ShouldReturnTotalIncomeForCurrentMonth()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);

			// Act
			var totalIncome = await repository.GetCurrentMonthTotalIncome(1); // Assuming user ID 1

			// Assert
			Assert.True(totalIncome >= 0);
		}

		[Fact]
		public async Task GetTotalIncomeForMonth_ShouldReturnTotalIncomeForSpecificMonth()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);
			var month = new DateTime(2022, 1, 1);

			// Act
			var totalIncome = await repository.GetTotalIncomeForMonth(month, 1); // Assuming user ID 1

			// Assert
			Assert.True(totalIncome >= 0);
		}

		[Fact]
		public async Task Update_ShouldUpdateExistingIncomeRecord()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);
			var income = new FinancialRecord
			{
				Title = "Test Income",
				Amount = 100,
				Description = "Test Description",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 1 // Assuming category ID 1 exists
			};
			await context.FinancialRecords.AddAsync(income);
			await context.SaveChangesAsync();

			// Act
			income.Title = "Updated Income Title";
			await repository.Update(income);

			// Assert
			var updatedIncome = await context.FinancialRecords.FindAsync(income.Id);
			Assert.Equal("Updated Income Title", updatedIncome.Title);
		}

		[Fact]
		public async Task Delete_ShouldDeleteExistingIncomeRecord()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);
			var income = new FinancialRecord
			{
				Title = "Test Income",
				Amount = 100,
				Description = "Test Description",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 1 // Assuming category ID 1 exists
			};
			await context.FinancialRecords.AddAsync(income);
			await context.SaveChangesAsync();

			// Act
			await repository.Delete(income.Id);

			// Assert
			var deletedIncome = await context.FinancialRecords.FindAsync(income.Id);
			Assert.Null(deletedIncome);
		}

		[Fact]
		public async Task GetTotalIncomeForCategory_ShouldReturnTotalIncomeForCategoryInDateRange()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);
			var startDate = new DateTime(2022, 1, 1);
			var endDate = new DateTime(2022, 1, 31);

			// Act
			var totalIncome = await repository.GetTotalIncomeForCategory(startDate, endDate, 1, 1); // Assuming user ID 1 and category ID 1

			// Assert
			Assert.True(totalIncome >= 0);
		}

		[Fact]
		public async Task GetHomeAll_ShouldReturnAllIncomeRecordsForHome()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);
			var homeId = 1;
			var userId = 1;

			var income1 = new FinancialRecord
			{
				Title = "Income 1",
				Amount = 200,
				Description = "Description 1",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 1,
				UserId = userId,
				HomeId = homeId
			};
			var income2 = new FinancialRecord
			{
				Title = "Income 2",
				Amount = 300,
				Description = "Description 2",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = 2,
				UserId = userId,
				HomeId = homeId
			};
			await context.FinancialRecords.AddRangeAsync(income1, income2);
			await context.SaveChangesAsync();

			// Act
			var allIncomes = await repository.GetHomeAll(homeId);

			// Assert
			Assert.NotNull(allIncomes);
			Assert.Equal(4, allIncomes.Count);
			Assert.True(allIncomes.All(i => i.HomeId == homeId));
		}

		[Fact]
		public async Task GetTotalHomeIncomeForMonth_ShouldReturnTotalIncomeForHomeForSpecificMonth()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new IncomeRepository(context);
			var homeId = 1;
			var userId = 1;
			var month = new DateTime(2022, 1, 1);

			var income1 = new FinancialRecord
			{
				Title = "Income 1",
				Amount = 200,
				Description = "Description 1",
				Time = month,
				Type = FinancialType.Income,
				FinancialCategoryId = 1,
				UserId = userId,
				HomeId = homeId
			};
			var income2 = new FinancialRecord
			{
				Title = "Income 2",
				Amount = 300,
				Description = "Description 2",
				Time = month,
				Type = FinancialType.Income,
				FinancialCategoryId = 2,
				UserId = userId,
				HomeId = homeId
			};
			await context.FinancialRecords.AddRangeAsync(income1, income2);
			await context.SaveChangesAsync();

			// Act
			var totalIncome = await repository.GetTotalHomeIncomeForMonth(month, homeId);

			// Assert
			Assert.Equal(500, totalIncome);
		}
	}
}
