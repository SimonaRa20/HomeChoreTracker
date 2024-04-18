using HomeChoreTracker.Api.Contracts.Gamification;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Controllers;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.Controllers
{
	public class GamificationControllerTests
	{
		private readonly GamificationController _gamificationController;
		private readonly Mock<IGamificationRepository> _gamificationRepositoryMock;
		private readonly Mock<IHomeRepository> _homeRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;

		public GamificationControllerTests()
		{
			_gamificationRepositoryMock = new Mock<IGamificationRepository>();
			_homeRepositoryMock = new Mock<IHomeRepository>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_gamificationController = new GamificationController(
				_gamificationRepositoryMock.Object,
				_homeRepositoryMock.Object,
				_userRepositoryMock.Object
			);
		}
		[Fact]
		public async Task AddGamificationLevel_Returns_OkResult()
		{
			// Arrange
			var imageStream = new MemoryStream();
			var imageFile = new FormFile(imageStream, 0, imageStream.Length, "image", "test.jpg");

			var levelRequest = new GamificationLevelRequest
			{
				LevelId = 1,
				PointsRequired = 100,
				Image = imageFile
			};

			_gamificationRepositoryMock.Setup(repo => repo.AddLevel(It.IsAny<GamificationLevel>()))
										.Returns(Task.CompletedTask);

			// Act
			var result = await _gamificationController.AddGamificationLevel(levelRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Level added successfully", okResult.Value);
		}

		[Fact]
		public async Task UpdateGamificationLevel_Returns_OkResult()
		{
			// Arrange
			int id = 1;
			var imageStream = new MemoryStream();
			var imageFile = new FormFile(imageStream, 0, imageStream.Length, "image", "test.jpg");

			var levelRequest = new GamificationLevelUpdateRequest
			{
				PointsRequired = 200,
				Image = imageFile
			};

			var gamificationLevel = new GamificationLevel { Id = id };

			_gamificationRepositoryMock.Setup(repo => repo.GetGamificationLevelById(id))
										.ReturnsAsync(gamificationLevel);
			_gamificationRepositoryMock.Setup(repo => repo.Update(It.IsAny<GamificationLevel>()))
										.Returns(Task.CompletedTask);

			// Act
			var result = await _gamificationController.UpdateGamificationLevel(id, levelRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Article with ID {id} updated successfully", okResult.Value);
		}

		[Fact]
		public async Task GetThisWeekPointsStatistic_Returns_OkResult()
		{
			// Arrange
			int homeId = 1;
			var pointsHistories = new List<PointsHistory>();
			var homers = new List<UserGetResponse>();

			_gamificationRepositoryMock.Setup(repo => repo.GetHomeThisWeekPointsHistory(homeId))
										.ReturnsAsync(pointsHistories);
			_homeRepositoryMock.Setup(repo => repo.GetHomeMembers(homeId))
										.ReturnsAsync(homers);

			// Act
			var result = await _gamificationController.GetThisWeekPointsStatistic(homeId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.IsType<Dictionary<string, int>>(okResult.Value);
		}

		[Fact]
		public async Task GetPreviousWeekPointsStatistic_Returns_OkResult()
		{
			// Arrange
			int homeId = 1;
			var pointsHistories = new List<PointsHistory>();
			var homers = new List<UserGetResponse>();

			_gamificationRepositoryMock.Setup(repo => repo.GetHomePreviousWeekPointsHistory(homeId))
										.ReturnsAsync(pointsHistories);
			_homeRepositoryMock.Setup(repo => repo.GetHomeMembers(homeId))
										.ReturnsAsync(homers);

			// Act
			var result = await _gamificationController.GetPreviousWeekPointsStatistic(homeId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.IsType<Dictionary<string, int>>(okResult.Value);
		}
	}
}
