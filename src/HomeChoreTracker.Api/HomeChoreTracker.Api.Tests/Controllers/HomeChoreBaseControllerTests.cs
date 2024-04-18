using AutoFixture;
using AutoMapper;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Controllers;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.Controllers
{
	public class HomeChoreBaseControllerTests
	{
		private readonly Fixture _fixture;
		private readonly HomeChoreBaseController _homeChoreBaseController;
		private readonly Mock<IHomeChoreBaseRepository> _homeChoreBaseRepositoryMock;
		private readonly Mock<IMapper> _mapperMock;
		public HomeChoreBaseControllerTests()
		{
			_fixture = new Fixture();
			_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			_mapperMock = new Mock<IMapper>();
			_homeChoreBaseRepositoryMock = new Mock<IHomeChoreBaseRepository>();
			_homeChoreBaseController = new HomeChoreBaseController(_homeChoreBaseRepositoryMock.Object, _mapperMock.Object);
		}


		[Fact]
		public async Task CreateHomeChoreBase_ShouldReturnOkResult_WhenSuccessfullyCreated()
		{
			// Arrange
			var homeChoreBaseRequest = _fixture.Create<HomeChoreBaseRequest>();

			// Act
			var result = await _homeChoreBaseController.CreateHomeChoreBase(homeChoreBaseRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Home chore base created successfully", okResult.Value);
		}

		[Fact]
		public async Task GetHomeChoreBase_ShouldReturnOkResult_WhenChoreBaseExists()
		{
			// Arrange
			var id = 1;
			var homeChoreBase = _fixture.Create<HomeChoreBase>();
			_homeChoreBaseRepositoryMock.Setup(repo => repo.GetChoreBase(id)).ReturnsAsync(homeChoreBase);

			// Act
			var result = await _homeChoreBaseController.GetHomeChoreBase(id);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(homeChoreBase, okResult.Value);
		}

		[Fact]
		public async Task UpdateHomeChoreBase_ShouldReturnOkResult_WhenSuccessfullyUpdated()
		{
			// Arrange
			var id = 1;
			var homeChoreBaseRequest = _fixture.Create<HomeChoreBaseRequest>();
			var homeChoreBase = _fixture.Create<HomeChoreBase>();
			_homeChoreBaseRepositoryMock.Setup(repo => repo.GetChoreBase(id)).ReturnsAsync(homeChoreBase);

			// Act
			var result = await _homeChoreBaseController.UpdateHomeChoreBase(id, homeChoreBaseRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} updated successfully", okResult.Value);
		}

		[Fact]
		public async Task DeleteHomeChoreBase_ShouldReturnOkResult_WhenSuccessfullyDeleted()
		{
			// Arrange
			var id = 1;

			// Act
			var result = await _homeChoreBaseController.DeleteHomeChoreBase(id);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} deleted successfully", okResult.Value);
		}

		
		[Fact]
		public async Task UpdateHomeChoreBase_ShouldReturnBadRequest_WhenChoreBaseNotFound()
		{
			// Arrange
			var id = 1;
			var homeChoreBaseRequest = _fixture.Create<HomeChoreBaseRequest>();
			_homeChoreBaseRepositoryMock.Setup(repo => repo.GetChoreBase(id)).ReturnsAsync((HomeChoreBase)null);

			// Act
			var result = await _homeChoreBaseController.UpdateHomeChoreBase(id, homeChoreBaseRequest);

			// Assert
			var badRequestResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} not found", badRequestResult.Value);
		}

		[Fact]
		public async Task UpdateHomeChoreBase_ShouldReturnOkResult_WhenDaysOfWeekIsNull()
		{
			// Arrange
			var id = 1;
			var homeChoreBaseRequest = _fixture.Build<HomeChoreBaseRequest>()
				.Without(x => x.DaysOfWeek)
				.Create();
			var homeChoreBase = _fixture.Create<HomeChoreBase>();
			_homeChoreBaseRepositoryMock.Setup(repo => repo.GetChoreBase(id)).ReturnsAsync(homeChoreBase);

			// Act
			var result = await _homeChoreBaseController.UpdateHomeChoreBase(id, homeChoreBaseRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} updated successfully", okResult.Value);
		}


		[Theory]
		[InlineData(new int[] { 0, 1, 2, 3 })]
		[InlineData(new int[] { 4, 5, 6, 7 })]
		public async Task UpdateHomeChoreBase_ShouldReturnOkResult_WhenDaysOfWeekIsNotEmpty(int[] days)
		{
			// Arrange
			var id = 1;
			var homeChoreBaseRequest = _fixture.Build<HomeChoreBaseRequest>()
				.With(x => x.DaysOfWeek, days.ToList())
				.Create();
			var homeChoreBase = _fixture.Create<HomeChoreBase>();
			_homeChoreBaseRepositoryMock.Setup(repo => repo.GetChoreBase(id)).ReturnsAsync(homeChoreBase);

			// Act
			var result = await _homeChoreBaseController.UpdateHomeChoreBase(id, homeChoreBaseRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} updated successfully", okResult.Value);
		}

		[Fact]
		public async Task GetHomeChoreBase_ShouldReturnNotFound_WhenChoreBaseDoesNotExist()
		{
			// Arrange
			var id = 1;
			_homeChoreBaseRepositoryMock.Setup(repo => repo.GetChoreBase(id)).ReturnsAsync((HomeChoreBase)null);

			// Act
			var result = await _homeChoreBaseController.GetHomeChoreBase(id);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} not found", notFoundResult.Value);
		}

		[Fact]
		public async Task DeleteHomeChoreBase_ShouldReturnNotFound_WhenChoreBaseDoesNotExist()
		{
			// Arrange
			var id = 1;
			_homeChoreBaseRepositoryMock.Setup(repo => repo.Delete(id)).ThrowsAsync(new KeyNotFoundException());

			// Act
			var result = await _homeChoreBaseController.DeleteHomeChoreBase(id);

			// Assert
			var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("An error occurred while deleting the article: The given key was not present in the dictionary.", notFoundResult.Value);
		}
	}
}
