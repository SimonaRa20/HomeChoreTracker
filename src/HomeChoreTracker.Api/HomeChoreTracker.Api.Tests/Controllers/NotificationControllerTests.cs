using AutoFixture;
using HomeChoreTracker.Api.Controllers;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.Controllers
{
	public class NotificationControllerTests
	{
		private readonly Fixture _fixture;
		private readonly NotificationController _notificationController;
		private readonly Mock<INotificationRepository> _notificationRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;

		public NotificationControllerTests()
		{
			_fixture = new Fixture();
			_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			_userRepositoryMock = new Mock<IUserRepository>();
			_notificationRepositoryMock = new Mock<INotificationRepository>();
			_notificationController = new NotificationController(_notificationRepositoryMock.Object, _userRepositoryMock.Object);
		}

		[Fact]
		public async Task SetRead_WhenNoNotifications_ReturnsOkResult()
		{
			// Arrange
			var userId = 1;
			_notificationController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};
			_notificationRepositoryMock.Setup(repo => repo.GetNotifications(userId)).ReturnsAsync((List<Notification>)null);

			// Act
			var result = await _notificationController.SetRead();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Not found notifications", okResult.Value);
		}

		[Fact]
		public async Task SetRead_WhenNotificationsExist_MarksAllAsReadAndReturnsOkResult()
		{
			// Arrange
			var userId = 1;
			_notificationController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};
			var notifications = new List<Notification>
			{
				new Notification { IsRead = false },
				new Notification { IsRead = true },
				new Notification { IsRead = false },
			};
			_notificationRepositoryMock.Setup(repo => repo.GetNotifications(userId)).ReturnsAsync(notifications.ToList());

			// Act
			var result = await _notificationController.SetRead();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Marked as read all notifications", okResult.Value);
		}
	}
}
