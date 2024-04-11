using AutoFixture;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
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
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests
{
	public class UserControllerTests
	{
		private readonly Fixture _fixture;
		private readonly UserController _userController;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IAuthRepository> _authRepositoryMock;
		private readonly Mock<IMapper> _mapperMock;

		public UserControllerTests()
		{
			_fixture = new Fixture();
			_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			_authRepositoryMock = new Mock<IAuthRepository>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_mapperMock = new Mock<IMapper>();
			_userController = new UserController(_userRepositoryMock.Object, _authRepositoryMock.Object, _mapperMock.Object);
		}


		[Fact]
		public async Task GetUserData_Returns_OkResult_With_UserData()
		{
			// Arrange
			var expectedUserId = 1;
			var expectedUser = _fixture.Create<User>();
			var expectedResponse = _fixture.Create<UserGetResponse>();

			_userRepositoryMock.Setup(repo => repo.GetUserById(expectedUserId))
				.ReturnsAsync(expectedUser);
			_mapperMock.Setup(mapper => mapper.Map<UserGetResponse>(expectedUser))
				.Returns(expectedResponse);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.Name, expectedUserId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.GetUserData();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var responseData = Assert.IsAssignableFrom<UserGetResponse>(okResult.Value);
			Assert.Equal(expectedResponse, responseData);
		}

		[Fact]
		public async Task GetUserBusyIntervals_Returns_OkResult_With_BusyIntervals()
		{
			// Arrange
			var expectedUserId = 1;
			var expectedBusyIntervals = _fixture.CreateMany<BusyInterval>().ToList();
			var expectedResponse = expectedBusyIntervals.Select(b => new BusyIntervalResponse
			{
				Id = b.Id,
				Day = b.Day,
				StartTime = b.StartTime,
				EndTime = b.EndTime
			}).OrderBy(r => r.Day).ThenBy(r => r.StartTime).ToList();

			_userRepositoryMock.Setup(repo => repo.GetUserBusyIntervals(expectedUserId))
				.ReturnsAsync(expectedBusyIntervals);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, expectedUserId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.GetUserBusyItervals();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var responseData = Assert.IsAssignableFrom<List<BusyIntervalResponse>>(okResult.Value);

			for (int i = 0; i < expectedResponse.Count; i++)
			{
				Assert.Equal(expectedResponse[i].Id, responseData[i].Id);
				Assert.Equal(expectedResponse[i].Day, responseData[i].Day);
				Assert.Equal(expectedResponse[i].StartTime, responseData[i].StartTime);
				Assert.Equal(expectedResponse[i].EndTime, responseData[i].EndTime);
			}
		}


		[Fact]
		public async Task AddBusyInterval_Returns_OkResult_When_Successfully_Added()
		{
			// Arrange
			var userId = 1;
			var busyIntervalRequest = _fixture.Create<BusyIntervalRequest>();
			var user = _fixture.Create<User>();
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.AddBusyInterval(busyIntervalRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Interval added successfully", okResult.Value);
			_userRepositoryMock.Verify(repo => repo.AddBusyInterval(It.IsAny<BusyInterval>()), Times.Once);
		}

		[Fact]
		public async Task DeleteBusyIntervalById_Returns_OkResult_When_Successfully_Deleted()
		{
			// Arrange
			var intervalId = 1;

			// Act
			var result = await _userController.DeleteBusyIntervalById(intervalId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Busy interval with ID {intervalId} deleted successfully", okResult.Value);
			_userRepositoryMock.Verify(repo => repo.DeleteInterval(intervalId), Times.Once);
		}

		[Fact]
		public async Task UpdateBusyInterval_Returns_OkResult_When_Successfully_Updated()
		{
			// Arrange
			var intervalId = 1;
			var adviceRequest = _fixture.Create<BusyIntervalRequest>();
			var userId = 1;
			var intervalToUpdate = _fixture.Create<BusyInterval>();
			intervalToUpdate.UserId = userId;
			_userRepositoryMock.Setup(repo => repo.GetBusyIntervalById(intervalId)).ReturnsAsync(intervalToUpdate);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.UpdateBusyInterval(intervalId, adviceRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Advice updated successfully", okResult.Value);
			_userRepositoryMock.Verify(repo => repo.UpdateInterval(It.IsAny<BusyInterval>()), Times.Once);
		}

		[Fact]
		public async Task UpdateUserData_Returns_OkResult_When_Successfully_Updated()
		{
			// Arrange
			var userId = 1;
			var updatedProfile = _fixture.Create<UserGetResponse>();
			updatedProfile.UserName = "UpdatedUserName";
			updatedProfile.Email = "updated@example.com";
			var user = _fixture.Create<User>();
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
			_authRepositoryMock.Setup(repo => repo.GetUserByEmail(updatedProfile.Email)).ReturnsAsync((User)null);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.UpdateUserData(updatedProfile);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(updatedProfile, okResult.Value);
			_userRepositoryMock.Verify(repo => repo.UpdateUser(It.IsAny<User>()), Times.Once);
		}


		[Fact]
		public async Task UpdateUserData_Returns_UnprocessableEntityResult_When_Username_IsEmpty()
		{
			// Arrange
			var userId = 1;
			var updatedProfile = _fixture.Create<UserGetResponse>();
			updatedProfile.UserName = string.Empty;
			updatedProfile.Email = "updated@example.com";
			var user = _fixture.Create<User>();
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
			_authRepositoryMock.Setup(repo => repo.GetUserByEmail(updatedProfile.Email)).ReturnsAsync((User)null);
			updatedProfile.UserName = string.Empty;

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.UpdateUserData(updatedProfile);

			// Assert
			var objectResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal("Username cannot be empty", objectResult.Value);
			Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult.StatusCode);
			_userRepositoryMock.Verify(repo => repo.UpdateUser(It.IsAny<User>()), Times.Never);
		}

		[Fact]
		public async Task UpdateUserData_Returns_UnprocessableEntityResult_When_Email_IsEmpty()
		{
			// Arrange
			var userId = 1;
			var updatedProfile = _fixture.Create<UserGetResponse>();
			updatedProfile.UserName ="test";
			updatedProfile.Email = "updated@example.com";
			var user = _fixture.Create<User>();
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
			_authRepositoryMock.Setup(repo => repo.GetUserByEmail(updatedProfile.Email)).ReturnsAsync((User)null);
			updatedProfile.Email = string.Empty;

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.UpdateUserData(updatedProfile);

			// Assert
			var objectResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal("Email cannot be empty", objectResult.Value);
			Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult.StatusCode);
			_userRepositoryMock.Verify(repo => repo.UpdateUser(It.IsAny<User>()), Times.Never);
		}

		[Fact]
		public async Task UpdateUserData_Returns_UnprocessableEntityResult_When_Email_IsAlreadyUsed()
		{
			// Arrange
			var userId = 1;
			var updatedProfile = _fixture.Create<UserGetResponse>();
			var existingUser = _fixture.Create<User>();
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(existingUser);
			_authRepositoryMock.Setup(repo => repo.GetUserByEmail(updatedProfile.Email)).ReturnsAsync(existingUser);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.UpdateUserData(updatedProfile);

			// Assert
			var objectResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal("Email is used already, change to another.", objectResult.Value);
			Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult.StatusCode);
			_userRepositoryMock.Verify(repo => repo.UpdateUser(It.IsAny<User>()), Times.Never);
		}

		[Fact]
		public async Task UpdateBusyInterval_Returns_NotFound_When_Interval_NotFound()
		{
			// Arrange
			var intervalId = 1;
			var adviceRequest = _fixture.Create<BusyIntervalRequest>();
			var userId = 1;

			_userRepositoryMock.Setup(repo => repo.GetBusyIntervalById(intervalId)).ReturnsAsync((BusyInterval)null);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.UpdateBusyInterval(intervalId, adviceRequest);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"Advice with ID {intervalId} not found", notFoundResult.Value);
		}

		[Fact]
		public async Task UpdateBusyInterval_Returns_Unauthorized_When_User_Does_Not_Have_Permission()
		{
			// Arrange
			var intervalId = 1;
			var adviceRequest = _fixture.Create<BusyIntervalRequest>();
			var userId = 1;
			var intervalToUpdate = _fixture.Create<BusyInterval>();
			intervalToUpdate.UserId = 999;
			_userRepositoryMock.Setup(repo => repo.GetBusyIntervalById(intervalId)).ReturnsAsync(intervalToUpdate);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.UpdateBusyInterval(intervalId, adviceRequest);

			// Assert
			var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
			Assert.Equal("You do not have permission to update this advice", unauthorizedResult.Value);
		}

		[Fact]
		public async Task UpdateUserData_Returns_NotFound_When_User_NotFound()
		{
			// Arrange
			var userId = 1;
			var updatedProfile = _fixture.Create<UserGetResponse>();

			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.UpdateUserData(updatedProfile);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task UpdateUserData_Returns_BadRequest_When_Exception_Occurs()
		{
			// Arrange
			var userId = 1;
			var updatedProfile = _fixture.Create<UserGetResponse>();
			var exceptionMessage = "Test exception message";

			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(new User());

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			_userRepositoryMock.Setup(repo => repo.UpdateUser(It.IsAny<User>())).ThrowsAsync(new Exception(exceptionMessage));

			// Act
			var result = await _userController.UpdateUserData(updatedProfile);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(exceptionMessage, badRequestResult.Value);
		}
		[Fact]
		public async Task AddBusyInterval_Returns_NotFound_When_User_NotFound()
		{
			// Arrange
			var userId = 1;
			var busyIntervalRequest = _fixture.Create<BusyIntervalRequest>();

			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.AddBusyInterval(busyIntervalRequest);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task GetUserData_Returns_NotFound_When_User_NotFound()
		{
			// Arrange
			var userId = 1;
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.GetUserData();

			// Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task GetUserData_Returns_BadRequest_When_Exception_Occurs()
		{
			// Arrange
			var userId = 1;
			var exceptionMessage = "Test exception message";

			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ThrowsAsync(new Exception(exceptionMessage));

			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _userController.GetUserData();

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(exceptionMessage, badRequestResult.Value);
		}

	}
}
