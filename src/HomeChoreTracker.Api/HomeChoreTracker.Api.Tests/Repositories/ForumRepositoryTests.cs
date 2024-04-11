using AutoMapper;
using FluentAssertions;
using HomeChoreTracker.Api.Database;
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
				Title = "mfioems",
				Time = DateTime.Now,
				Type = Constants.HomeChoreType.Laundry,
				Description = "esfsfr",
				IsPublic = false,
				UserId = 1
			});
			context.SaveChanges();

			var advice = await repository.GetAdviceById(1);

			advice.Id.Should().Be(1);
		}
	}
}
