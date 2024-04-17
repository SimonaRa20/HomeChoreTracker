using HomeChoreTracker.Api.Contracts.Home;
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
	public class HomeRepositoryTests
	{
		private readonly HomeChoreTrackerDbFixture _homeChoreTrackerDbFixture;

		public HomeRepositoryTests(HomeChoreTrackerDbFixture trakiDbFixture)
		{
			_homeChoreTrackerDbFixture = trakiDbFixture;
		}

		[Fact]
		public async Task GetInvitationByToken_ShouldReturnInvitation()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepository = new UserRepository(context);
			var gamificationRepository = new GamificationRepository(context, userRepository);
			var repository = new HomeRepository(context, userRepository, gamificationRepository);
			var token = "test-token";

			// Act
			var invitation = await repository.GetInvitationByToken(token);

			// Assert
			Assert.Null(invitation);
		}

		[Fact]
		public async Task AddToHome_ShouldAddUserToHome()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepository = new UserRepository(context);
			var gamificationRepository = new GamificationRepository(context, userRepository);
			var repository = new HomeRepository(context, userRepository, gamificationRepository);
			var userHomes = new UserHomes
			{
				UserId = 1,
				HomeId = 1
			};

			// Act
			await repository.AddToHome(userHomes);
			var addedUserHome = await context.UserHomes.FirstOrDefaultAsync();

			// Assert
			Assert.NotNull(addedUserHome);
			Assert.Equal(userHomes.UserId, addedUserHome.UserId);
			Assert.Equal(userHomes.HomeId, addedUserHome.HomeId);
		}

		[Fact]
		public async Task RemoveInvitation_ShouldRemoveInvitation()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepository = new UserRepository(context);
			var gamificationRepository = new GamificationRepository(context, userRepository);
			var repository = new HomeRepository(context, userRepository, gamificationRepository);
			var invitation = new HomeInvitation
			{
				HomeId = 1,
				InviterUserId = 1,
				InviteeEmail = "test@example.com",
				InvitationToken = "test-token",
				ExpirationDate = DateTime.UtcNow.AddDays(7)
			};
			await context.HomeInvitations.AddAsync(invitation);
			await context.SaveChangesAsync();

			// Act
			await repository.RemoveInvitation(invitation);
			var removedInvitation = await context.HomeInvitations.FirstOrDefaultAsync();

			// Assert
			Assert.Null(removedInvitation);
		}

		[Fact]
		public async Task CheckOrExistTitle_ShouldReturnTrueIfTitleExists()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepository = new UserRepository(context);
			var gamificationRepository = new GamificationRepository(context, userRepository);
			var repository = new HomeRepository(context, userRepository, gamificationRepository);
			var homeRequest = new HomeRequest
			{
				Title = "Test Home"
			};
			await context.Homes.AddAsync(new Home { Title = "Test Home" });
			await context.SaveChangesAsync();

			// Act
			var exists = await repository.CheckOrExistTitle(homeRequest);

			// Assert
			Assert.True(exists);
		}
	}
}
