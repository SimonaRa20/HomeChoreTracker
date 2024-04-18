using HomeChoreTracker.Api.Contracts.Forum;
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
	public class ForumControllerTests
	{
		private readonly ForumController _forumController;
		private readonly Mock<IForumRepository> _forumRepositoryMock;
		private readonly Mock<IIncomeRepository> _incomeRepositoryMock;
		private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
		private readonly Mock<IGamificationRepository> _gamificationRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<INotificationRepository> _notificationRepositoryMock;
		private readonly Mock<IHomeRepository> _homeRepositoryMock;

		public ForumControllerTests()
		{
			_forumRepositoryMock = new Mock<IForumRepository>();
			_incomeRepositoryMock = new Mock<IIncomeRepository>();
			_expenseRepositoryMock = new Mock<IExpenseRepository>();
			_gamificationRepositoryMock = new Mock<IGamificationRepository>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_notificationRepositoryMock = new Mock<INotificationRepository>();
			_homeRepositoryMock = new Mock<IHomeRepository>();

			_forumController = new ForumController(
				_forumRepositoryMock.Object,
				_userRepositoryMock.Object,
				_gamificationRepositoryMock.Object,
				_notificationRepositoryMock.Object
			);
		}

		[Fact]
		public async Task AddAdvice_Returns_OkResult()
		{
			// Arrange
			var adviceRequest = new AdviceRequest
			{
				Title = "Test Advice",
				Type = Constants.HomeChoreType.Cleaning,
				Description = "Test Description",
				IsPublic = true
			};
			int userId = 1;
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(new User());
			_gamificationRepositoryMock.Setup(repo => repo.UserHasCreateFirstAdviceBadge(userId)).ReturnsAsync(false);
			_forumRepositoryMock.Setup(repo => repo.AddAdvice(It.IsAny<Advice>())).Returns(Task.CompletedTask);
			_gamificationRepositoryMock.Setup(repo => repo.GetUserBadgeWallet(userId)).ReturnsAsync(new BadgeWallet());
			_notificationRepositoryMock.Setup(repo => repo.CreateNotification(It.IsAny<Notification>())).Returns(Task.CompletedTask);
			var userClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, "1")
			};
			var userIdentity = new ClaimsIdentity(userClaims, "TestAuth");
			var userPrincipal = new ClaimsPrincipal(userIdentity);
			_forumController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = userPrincipal }
			};
			// Act
			var result = await _forumController.AddAdvice(adviceRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Advice added successfully", okResult.Value);
		}

		[Fact]
		public async Task DeleteAdviceById_Returns_OkResult()
		{
			// Arrange
			int adviceId = 1;
			_forumRepositoryMock.Setup(repo => repo.Delete(adviceId)).Returns(Task.CompletedTask);

			// Act
			var result = await _forumController.DeleteAdviceById(adviceId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Advice with ID {adviceId} deleted successfully", okResult.Value);
		}

		[Fact]
		public async Task GetAdvice_Returns_OkResult_With_Advice()
		{
			// Arrange
			int adviceId = 1;
			var advice = new Advice { Id = adviceId };
			_forumRepositoryMock.Setup(repo => repo.GetAdviceById(adviceId)).ReturnsAsync(advice);

			// Act
			var result = await _forumController.GetAdvice(adviceId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedAdvice = Assert.IsType<Advice>(okResult.Value);
			Assert.Equal(adviceId, returnedAdvice.Id);
		}
		[Fact]
		public async Task UpdateAdvice_Returns_OkResult()
		{
			// Arrange
			int adviceId = 1;
			var adviceRequest = new AdviceRequest { Title = "Updated Title" };
			var adviceToUpdate = new Advice { Id = adviceId, UserId = 1 }; // Create a mock Advice object
			_forumRepositoryMock.Setup(repo => repo.GetAdviceById(adviceId)).ReturnsAsync(adviceToUpdate);
			_forumRepositoryMock.Setup(repo => repo.UpdateAdvice(It.IsAny<Advice>())).Returns(Task.CompletedTask);

			var userClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, "1")
			};
			var userIdentity = new ClaimsIdentity(userClaims, "TestAuth");
			var userPrincipal = new ClaimsPrincipal(userIdentity);
			_forumController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = userPrincipal }
			};

			// Act
			var result = await _forumController.UpdateAdvice(adviceId, adviceRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Advice updated successfully", okResult.Value);
		}

	}
}
