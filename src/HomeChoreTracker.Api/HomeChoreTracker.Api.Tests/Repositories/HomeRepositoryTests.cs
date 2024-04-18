using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using HomeChoreTracker.Api.Tests.DatabaseFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
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

		[Fact]
		public async Task Get_ShouldReturnHomeInvitation()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepository = new UserRepository(context);
			var gamificationRepository = new GamificationRepository(context, userRepository);
			var repository = new HomeRepository(context, userRepository, gamificationRepository);
			var homeId = 1;
			await context.HomeInvitations.AddAsync(new HomeInvitation { HomeId = homeId, InvitationToken = "token", InviteeEmail = "test@gmail.com" });
			await context.SaveChangesAsync();

			// Act
			var invitation = await repository.Get(homeId);

			// Assert
			Assert.NotNull(invitation);
		}

		[Fact]
		public async Task InviteUserToHome_ShouldReturnInvitationToken()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepository = new Mock<IUserRepository>();
			var gamificationRepository = new GamificationRepository(context, userRepository.Object);
			var repository = new HomeRepository(context, userRepository.Object, gamificationRepository);
			var inviterUserId = 1;
			var homeId = 1;
			var inviteeEmail = "test@example.com";

			// Act
			var token = await repository.InviteUserToHome(inviterUserId, homeId, inviteeEmail);

			// Assert
			Assert.NotNull(token);
		}


		[Fact]
		public async Task CheckOrExistTitle_ShouldReturnFalseIfTitleDoesNotExist()
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

			// Act
			var exists = await repository.CheckOrExistTitle(homeRequest);

			// Assert
			Assert.True(exists);
		}

		[Fact]
		public async Task Update_ShouldUpdateHome()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var userRepository = new UserRepository(context);
			var gamificationRepository = new GamificationRepository(context, userRepository);
			var repository = new HomeRepository(context, userRepository, gamificationRepository);
			var home = new Home { Title = "Old Title" };
			await context.Homes.AddAsync(home);
			await context.SaveChangesAsync();

			// Act
			home.Title = "New Title";
			await repository.Update(home);
			var updatedHome = await context.Homes.FindAsync(home.Id);

			// Assert
			Assert.Equal("New Title", updatedHome.Title);
		}

	}
}
