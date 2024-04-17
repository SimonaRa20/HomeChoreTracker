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
	public class NotificationRepositoryTests
	{
		private readonly HomeChoreTrackerDbFixture _homeChoreTrackerDbFixture;

		public NotificationRepositoryTests(HomeChoreTrackerDbFixture trakiDbFixture)
		{
			_homeChoreTrackerDbFixture = trakiDbFixture;
		}

		[Fact]
		public async Task CreateNotification_ShouldAddNotificationToDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new NotificationRepository(context);
			var notification = new Notification
			{
				Title = "Test Notification",
				IsRead = false,
				Time = DateTime.Now,
				UserId = 1 // Assuming user ID is 1
			};

			// Act
			await repository.CreateNotification(notification);

			// Assert
			var addedNotification = await context.Notifications.FirstOrDefaultAsync(n => n.Title == notification.Title);
			Assert.NotNull(addedNotification);
			Assert.Equal(notification.Title, addedNotification.Title);
		}

		[Fact]
		public async Task Update_ShouldUpdateNotificationInDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new NotificationRepository(context);
			var notification = new Notification
			{
				Title = "Test Notification",
				IsRead = false,
				Time = DateTime.Now,
				UserId = 1
			};
			await context.Notifications.AddAsync(notification);
			await context.SaveChangesAsync();

			// Modify notification
			notification.IsRead = true;

			// Act
			await repository.Update(notification);

			// Assert
			var updatedNotification = await context.Notifications.FirstOrDefaultAsync(n => n.Id == notification.Id);
			Assert.NotNull(updatedNotification);
			Assert.True(updatedNotification.IsRead);
		}
	}
}
