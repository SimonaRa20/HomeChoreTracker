using AutoFixture;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.Controllers
{
	public class HomeChoreControllerTests
	{
		private readonly Fixture _fixture;
		private readonly HomeChoreController _homeChoreController;
		private readonly Mock<IHomeChoreRepository> _homeChoreRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IPurchaseRepository> _purchaseRepositoryMock;
		private readonly Mock<IGamificationRepository> _gamificationRepositoryMock;
		private readonly Mock<INotificationRepository> _notificationRepositoryMock;
		private readonly Mock<IHomeChoreBaseRepository> _homeChoreBaseRepositoryMock;
		private readonly Mock<IHomeRepository> _homeRepositoryMock;
		private readonly Mock<IChallengeRepository> _challengeRepositoryMock;
		private readonly Mock<IMapper> _mapperMock;

		public HomeChoreControllerTests()
		{
			_fixture = new Fixture();
			_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			_userRepositoryMock = new Mock<IUserRepository>();
			_homeChoreRepositoryMock = new Mock<IHomeChoreRepository>();
			_purchaseRepositoryMock = new Mock<IPurchaseRepository>();
			_gamificationRepositoryMock = new Mock<IGamificationRepository>();
			_notificationRepositoryMock = new Mock<INotificationRepository>();
			_homeChoreBaseRepositoryMock = new Mock<IHomeChoreBaseRepository>();
			_challengeRepositoryMock = new Mock<IChallengeRepository>(); 
			_homeRepositoryMock = new Mock<IHomeRepository>();
			_mapperMock = new Mock<IMapper>();
			_homeChoreController = new HomeChoreController(_homeChoreRepositoryMock.Object, _purchaseRepositoryMock.Object,
														   _gamificationRepositoryMock.Object, _mapperMock.Object,
														   _userRepositoryMock.Object, _notificationRepositoryMock.Object,
														   _homeChoreBaseRepositoryMock.Object, _homeRepositoryMock.Object,
														   _challengeRepositoryMock.Object);
		}

		[Fact]
		public async Task AddHomeChore_ReturnsOkResult_When_GetHomeMembers_ReturnsValues()
		{
			// Arrange
			int homeId = 1;
			int taskId = 1;
			int userId = 1;

			var homeMembers = new List<UserGetResponse>
	{
		new UserGetResponse { HomeMemberId = userId, UserName = "TestUser", Email = "test@example.com" }
	};

			_homeRepositoryMock.Setup(repo => repo.GetHomeMembers(homeId)).ReturnsAsync(homeMembers);
			_homeRepositoryMock.Setup(repo => repo.OrHomeMember(homeId, userId)).ReturnsAsync(true);

			var homeChoreBase = _fixture.Create<HomeChoreBase>();

			homeChoreBase.UserId = userId;

			_homeChoreBaseRepositoryMock.Setup(repo => repo.GetChoreBase(taskId)).ReturnsAsync(homeChoreBase);
			_homeChoreRepositoryMock.Setup(repo => repo.AddHomeChoreBase(homeChoreBase, homeId));

			var user = new User { Id = userId, UserName = "TestUser", Email = "test@example.com" };
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

			_homeChoreController.ControllerContext = new ControllerContext
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
			var result = await _homeChoreController.AddHomeChore(homeId, taskId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Home chore was added successfully", okResult.Value);
		}


		[Fact]
		public async Task GetHomeChores_ReturnsOkResult()
		{
			// Arrange
			int homeId = 1;
			int userId = 1;

			_homeChoreController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			var homeChores = _fixture.CreateMany<HomeChoreTask>(3).ToList(); // Create a list of sample home chores

			_homeRepositoryMock.Setup(repo => repo.OrHomeMember(homeId, userId)).ReturnsAsync(true);
			_homeChoreRepositoryMock.Setup(repo => repo.GetAll(homeId)).ReturnsAsync(homeChores);

			// Act
			var result = await _homeChoreController.GetHomeChores(homeId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedHomeChores = Assert.IsAssignableFrom<IEnumerable<HomeChoreTask>>(okResult.Value);
			Assert.Equal(homeChores.Count, returnedHomeChores.Count());
		}

		[Fact]
		public async Task DeleteHomeChore_ReturnsOkResult_When_TaskNotAssigned()
		{
			// Arrange
			int taskId = 1;

			_homeChoreRepositoryMock.Setup(repo => repo.CheckOrHomeChoreWasAssigned(taskId)).ReturnsAsync(false);
			_homeChoreRepositoryMock.Setup(repo => repo.DeleteNotAssignedTasks(taskId)).Verifiable();
			_homeChoreRepositoryMock.Setup(repo => repo.Delete(taskId)).Verifiable();

			// Act
			var result = await _homeChoreController.DeleteHomeChore(taskId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Home chore base with ID {taskId} deleted successfully", okResult.Value);
		}

		[Fact]
		public async Task GetHomeChore_ReturnsOkResult_When_ChoreFound()
		{
			// Arrange
			int taskId = 1;
			var homeChore = _fixture.Create<HomeChoreTask>();

			_homeChoreRepositoryMock.Setup(repo => repo.Get(taskId)).ReturnsAsync(homeChore);

			// Act
			var result = await _homeChoreController.GetHomeChore(taskId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedHomeChore = Assert.IsType<HomeChoreTask>(okResult.Value);
			Assert.Equal(homeChore, returnedHomeChore);
		}

		[Fact]
		public async Task GetHomeChoresToCalendar_ReturnsOkResult_When_ChoresFound()
		{
			// Arrange
			int homeId = 1;
			int userId = 1;
			var taskAssignmentsResponse = _fixture.CreateMany<TaskAssignmentResponse>(3).ToList(); // Create a list of sample task assignments

			// Set up the user context
			_homeChoreController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			_homeRepositoryMock.Setup(repo => repo.OrHomeMember(homeId, It.IsAny<int>())).ReturnsAsync(true);

			var taskAssignments = taskAssignmentsResponse.Select(task =>
				new TaskAssignment
				{
					Id = task.Id,
					StartDate = task.StartDate,
					EndDate = task.EndDate,
					TaskId = task.TaskId,
					HomeMemberId = task.HomeMemberId,
					HomeId = task.HomeId,
					IsDone = task.IsDone,
					IsApproved = task.IsApproved,
				}).ToList();

			_homeChoreRepositoryMock.Setup(repo => repo.GetCalendar(homeId)).ReturnsAsync(taskAssignments);

			// Act
			var result = await _homeChoreController.GetHomeChoresToCalendar(homeId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedTaskAssignments = Assert.IsAssignableFrom<IEnumerable<TaskAssignmentResponse>>(okResult.Value);
			Assert.Equal(taskAssignmentsResponse.Count, returnedTaskAssignments.Count());
		}

		[Fact]
		public async Task GetHomeChore_ReturnsOkResult_When_HomeChoreFound()
		{
			// Arrange
			int id = 1;
			bool isDone = true;
			var homeChore = new TaskAssignment { Id = id, IsDone = isDone, HomeMemberId = 123 }; // Assuming HomeMemberId is set to a valid value
			var task = new HomeChoreTask { Id = 456 };
			var user = new User { Id = 123, BadgeWallet = new BadgeWallet() };
			var wallet = new BadgeWallet
			{
				DoneFirstTask = false,
				DoneFirstCleaningTask = false,
				DoneFirstLaundryTask = false,
				DoneFirstKitchenTask = false,
				DoneFirstBathroomTask = false,
				DoneFirstBedroomTask = false,
				DoneFirstOutdoorsTask = false,
				DoneFirstOrganizeTask = false,

				EarnedPerDayFiftyPoints = false,
				EarnedPerDayHundredPoints = false,

				DoneFiveTaskPerWeek = false,
				DoneTenTaskPerWeek = false,
				DoneTwentyFiveTaskPerWeek = false,

				CreatedTaskWasUsedOtherHome = false,

				CreateFirstPurchase = false,
				CreateFirstAdvice = false,

				CreateFirstIncome = false,
				CreateFirstExpense = false,
				UserId = 1
			};
			_homeChoreRepositoryMock.Setup(repo => repo.GetTaskAssigment(id)).ReturnsAsync(homeChore);
			_homeChoreRepositoryMock.Setup(repo => repo.Get(homeChore.TaskId)).ReturnsAsync(task);
			_userRepositoryMock.Setup(repo => repo.GetUserById(homeChore.HomeMemberId.Value)).ReturnsAsync(user);
			_gamificationRepositoryMock.Setup(repo => repo.GetUserBadgeWallet(user.Id)).ReturnsAsync(wallet); // Mocking the repository method to return the wallet object

			// Act
			var result = await _homeChoreController.GetHomeChore(id, isDone);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedHomeChore = Assert.IsType<TaskAssignment>(okResult.Value);
			Assert.Equal(homeChore.Id, returnedHomeChore.Id);
			Assert.Equal(homeChore.IsDone, returnedHomeChore.IsDone);
		}


		[Fact]
		public async Task GetHomeChore_ReturnsNotFoundResult_When_HomeChoreNotFound()
		{
			// Arrange
			int id = 1;
			bool isDone = true;
			_homeChoreRepositoryMock.Setup(repo => repo.GetTaskAssigment(id)).ReturnsAsync((TaskAssignment)null);

			// Act
			var result = await _homeChoreController.GetHomeChore(id, isDone);

			// Assert
			var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("An error occurred while fetching home chore bases: Object reference not set to an instance of an object.", notFoundResult.Value);
		}

		[Fact]
		public async Task GetHomeChore_ReturnsBadRequestResult_When_ExceptionThrown()
		{
			// Arrange
			int id = 1;
			bool isDone = true;
			_homeChoreRepositoryMock.Setup(repo => repo.GetTaskAssigment(id)).ThrowsAsync(new System.Exception());

			// Act
			var result = await _homeChoreController.GetHomeChore(id, isDone);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("An error occurred while fetching home chore bases: Exception of type 'System.Exception' was thrown.", badRequestResult.Value);
		}

		[Fact]
		public async Task UpdateHomeChore_ReturnsNotFound_When_HomeChoreNotFound()
		{
			// Arrange
			int id = 1;
			int userId = 123;
			_homeChoreRepositoryMock.Setup(repo => repo.Get(id)).ReturnsAsync((HomeChoreTask)null);
			_homeChoreController.ControllerContext = new ControllerContext
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
			var result = await _homeChoreController.UpdateHomeChore(id, new HomeChoreBaseRequest());

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} not found", notFoundResult.Value);
		}

		[Fact]
		public async Task UpdateHomeChore_ReturnsOk_When_HomeChoreUpdatedSuccessfully()
		{
			// Arrange
			int id = 1;
			int userId = 123;
			var homeChore = new HomeChoreTask { Id = id, StartDate = DateTime.Now.AddDays(-6), EndDate = DateTime.Now };

			var validRequest = new HomeChoreBaseRequest
			{
				Name = "Sample Name",
				ChoreType = HomeChoreType.Laundry,
				Description = "Sample Description",
				Points = 10,
				LevelType = LevelType.Medium,
				Time = TimeLong.hour,
				Interval = 1,
				Unit = RepeatUnit.Day,
				DaysOfWeek = new List<int> { 1, 2, 3 },
				DayOfMonth = 15,
				MonthlyRepeatType = MonthlyRepeatType.DayOfMonth,
				StartDate = DateTime.Now.AddDays(-6),
				EndDate = DateTime.Now
			};

			_homeChoreRepositoryMock.Setup(repo => repo.Get(id)).ReturnsAsync(homeChore);
			_homeChoreRepositoryMock.Setup(repo => repo.Update(It.IsAny<HomeChoreTask>()));
			_homeChoreController.ControllerContext = new ControllerContext
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
			var result = await _homeChoreController.UpdateHomeChore(id, validRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} updated successfully", okResult.Value);
		}


		[Fact]
		public async Task UpdateHomeChoreOrganize_ReturnsOk_When_HomeChoreUpdatedSuccessfully()
		{
			// Arrange
			int id = 1;
			int userId = 123;
			var homeChore = new HomeChoreTask { Id = id, StartDate = DateTime.Now.AddDays(-6), EndDate = DateTime.Now };

			var validRequest = new HomeChoreBaseRequest
			{
				Name = "Sample Name",
				ChoreType = HomeChoreType.Organize,
				Description = "Sample Description",
				Points = 10,
				LevelType = LevelType.Medium,
				Time = TimeLong.hour,
				Interval = 1,
				Unit = RepeatUnit.Week,
				DaysOfWeek = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 },
				DayOfMonth = 15,
				MonthlyRepeatType = MonthlyRepeatType.DayOfMonth,
				StartDate = DateTime.Now.AddDays(-6),
				EndDate = DateTime.Now,
			};

			_homeChoreRepositoryMock.Setup(repo => repo.Get(id)).ReturnsAsync(homeChore);
			_homeChoreRepositoryMock.Setup(repo => repo.Update(It.IsAny<HomeChoreTask>()));
			_homeChoreController.ControllerContext = new ControllerContext
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
			var result = await _homeChoreController.UpdateHomeChore(id, validRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} updated successfully", okResult.Value);
		}

		[Fact]
		public async Task UpdateHomeChoreMonth_ReturnsOk_When_HomeChoreUpdatedSuccessfully()
		{
			// Arrange
			int id = 1;
			int userId = 123;
			var homeChore = new HomeChoreTask { Id = id, StartDate = DateTime.Now.AddDays(-6), EndDate = DateTime.Now };

			var validRequest = new HomeChoreBaseRequest
			{
				Name = "Sample Name",
				ChoreType = HomeChoreType.Organize,
				Description = "Sample Description",
				Points = 10,
				LevelType = LevelType.Medium,
				Time = TimeLong.hour,
				Interval = 1,
				Unit = RepeatUnit.Month,
				DaysOfWeek = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 },
				DayOfMonth = 15,
				MonthlyRepeatType = MonthlyRepeatType.DayOfMonth,
				StartDate = DateTime.Now.AddDays(-6),
				EndDate = DateTime.Now,
			};

			_homeChoreRepositoryMock.Setup(repo => repo.Get(id)).ReturnsAsync(homeChore);
			_homeChoreRepositoryMock.Setup(repo => repo.Update(It.IsAny<HomeChoreTask>()));
			_homeChoreController.ControllerContext = new ControllerContext
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
			var result = await _homeChoreController.UpdateHomeChore(id, validRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} updated successfully", okResult.Value);
		}

		[Fact]
		public async Task UpdateHomeChoreYear_ReturnsOk_When_HomeChoreUpdatedSuccessfully()
		{
			// Arrange
			int id = 1;
			int userId = 123;
			var homeChore = new HomeChoreTask { Id = id, StartDate = DateTime.Now.AddDays(-6), EndDate = DateTime.Now };

			var validRequest = new HomeChoreBaseRequest
			{
				Name = "Sample Name",
				ChoreType = HomeChoreType.Organize,
				Description = "Sample Description",
				Points = 10,
				LevelType = LevelType.Medium,
				Time = TimeLong.hour,
				Interval = 1,
				Unit = RepeatUnit.Year,
				DaysOfWeek = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 },
				DayOfMonth = 15,
				MonthlyRepeatType = MonthlyRepeatType.DayOfMonth,
				StartDate = DateTime.Now.AddDays(-6),
				EndDate = DateTime.Now,
			};

			_homeChoreRepositoryMock.Setup(repo => repo.Get(id)).ReturnsAsync(homeChore);
			_homeChoreRepositoryMock.Setup(repo => repo.Update(It.IsAny<HomeChoreTask>()));
			_homeChoreController.ControllerContext = new ControllerContext
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
			var result = await _homeChoreController.UpdateHomeChore(id, validRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Home chore base with ID {id} updated successfully", okResult.Value);
		}
		[Fact]
		public async Task VoteArticle_ReturnsOkResult_When_VoteTaskIsSuccessful()
		{
			// Arrange
			int taskId = 1;
			int voteValue = 1;
			int userId = 123;

			var userClaims = new Claim[]
			{
				new Claim(ClaimTypes.Name, userId.ToString())
			};
			var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "mock"));

			_homeChoreRepositoryMock.Setup(repo => repo.VoteArtical(taskId, userId, voteValue)).ReturnsAsync(true);
			_homeChoreRepositoryMock.Setup(repo => repo.GetTaskAssigment(taskId)).ReturnsAsync(new TaskAssignment());
			_homeChoreRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync(new HomeChoreTask());
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(new User());
			_homeRepositoryMock.Setup(repo => repo.GetHomeMembers(It.IsAny<int>())).ReturnsAsync(new List<UserGetResponse>());

			_homeChoreController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = userPrincipal
				}
			};

			// Act
			var result = await _homeChoreController.VoteArticle(taskId, voteValue);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.True((bool)okResult.Value);
		}

		[Fact]
		public async Task VoteArticle_ReturnsBadRequestResult_When_ExceptionThrown()
		{
			// Arrange
			int taskId = 1;
			int voteValue = 1;
			int userId = 123;

			var userClaims = new Claim[]
			{
				new Claim(ClaimTypes.Name, userId.ToString())
			};
			var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "mock"));

			_homeChoreRepositoryMock.Setup(repo => repo.VoteArtical(taskId, userId, voteValue)).ThrowsAsync(new Exception());

			_homeChoreController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = userPrincipal
				}
			};

			// Act
			var result = await _homeChoreController.VoteArticle(taskId, voteValue);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		}
	}
}
