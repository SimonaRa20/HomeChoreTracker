using AutoMapper;
using FluentAssertions;
using HomeChoreTracker.Api.Database;
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
	public class ForumRepositoryTests
	{
		private readonly HomeChoreTrackerDbFixture _homeChoreTrackerDbFixture;

		public ForumRepositoryTests(HomeChoreTrackerDbFixture trakiDbFixture)
		{
			_homeChoreTrackerDbFixture = trakiDbFixture;
		}

		[Fact]
		public async Task GetAdvice_ReturnsAdvice()
		{
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new ForumRepository(context);
			context.Advices.Add(new Models.Advice
			{
				Id = 1,
				Title = "Advice1",
				Time = DateTime.Now,
				Type = Constants.HomeChoreType.Laundry,
				Description = "Advice1 description",
				IsPublic = false,
				UserId = 1
			});
			context.SaveChanges();

			var advice = await repository.GetAdviceById(1);

			advice.Id.Should().Be(1);
		}

		[Fact]
		public async Task AddAdvice_ShouldAddAdviceToDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new ForumRepository(context);
			var adviceToAdd = new Advice
			{
				Id = 2,
				Title = "Advice2",
				Time = DateTime.Now,
				Type = Constants.HomeChoreType.Laundry,
				Description = "Advice2 description",
				IsPublic = false,
				UserId = 1
			};

			// Act
			await repository.AddAdvice(adviceToAdd);

			// Assert
			var addedAdvice = await repository.GetAdviceById(adviceToAdd.Id);
			Assert.NotNull(addedAdvice);
		}

		[Fact]
		public async Task Delete_ShouldDeleteAdviceFromDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new ForumRepository(context);
			var adviceToDelete = new Advice
			{
				Title = "Advice3",
				Time = DateTime.Now,
				Type = Constants.HomeChoreType.Laundry,
				Description = "Advice3 description",
				IsPublic = false,
				UserId = 1
			};
			await repository.AddAdvice(adviceToDelete);

			// Act
			await repository.Delete(adviceToDelete.Id);

			// Assert
			var deletedAdvice = await repository.GetAdviceById(adviceToDelete.Id);
			Assert.Null(deletedAdvice);
		}

		[Fact]
		public async Task Update_ShouldUpdateAdviceFromDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new ForumRepository(context);
			var advice = new Advice
			{
				Id = 3,
				Title = "Advice3",
				Time = DateTime.Now,
				Type = Constants.HomeChoreType.Laundry,
				Description = "Advice3 description",
				IsPublic = false,
				UserId = 1
			};

			var checkAdvice = advice;

			await repository.AddAdvice(advice);

			advice.Title = "Mandarinas";

			// Act
			await repository.UpdateAdvice(advice);

			// Assert
			var updatedAdvice = await repository.GetAdviceById(advice.Id);

			Assert.Equal("Mandarinas", updatedAdvice.Title);
		}
	}
}
