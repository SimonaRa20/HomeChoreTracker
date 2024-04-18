using AutoFixture;
using AutoMapper;
using HomeChoreTracker.Api.Contracts.Calendar;
using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Controllers;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
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
	public class CalendarControllerTests
	{
		private readonly Fixture _fixture;
		private readonly CalendarController _calendarController;
		private readonly Mock<ICalendarRepository> _calendarRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IHomeChoreRepository> _homeChoreRepositoryMock;
		private readonly Mock<IPurchaseRepository> _purchaseRepositoryMock;

		public CalendarControllerTests()
		{
			_fixture = new Fixture();
			_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			_userRepositoryMock = new Mock<IUserRepository>();
			_calendarRepositoryMock = new Mock<ICalendarRepository>();
			_homeChoreRepositoryMock = new Mock<IHomeChoreRepository>();
			_purchaseRepositoryMock = new Mock<IPurchaseRepository>();
			_calendarController = new CalendarController(_calendarRepositoryMock.Object, _homeChoreRepositoryMock.Object, _userRepositoryMock.Object,_purchaseRepositoryMock.Object);
		}

		[Fact]
		public async Task AddBusyInterval_Returns_BadRequestObjectResult_When_Null_Added()
		{
			// Arrange
			var userId = 1;
			var busyIntervalRequest = _fixture.Create<BusyIntervalRequest>();
			var user = _fixture.Create<User>();
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

			_calendarController.ControllerContext = new ControllerContext
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
			var result = await _calendarController.AssignTasksToMembers(1,true, true,true);

			// Assert
			var okResult = Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task AssignTasksToMembers_Returns_BadRequestObjectResult_When_UnassignedTasksIsNull()
		{
			// Arrange
			int homeId = 1;
			bool googleCheck = true;
			bool busyIntervalCheck = true;
			bool assignedHomeChoresCheck = true;

			var userRepositoryMock = new Mock<IUserRepository>();
			var homeChoreRepositoryMock = new Mock<IHomeChoreRepository>();

			var userId = 1;
			var user = new User { Id = userId };
			userRepositoryMock.Setup(repo => repo.GetHomeMembers(homeId)).ReturnsAsync(new List<User> { user });
			userRepositoryMock.Setup(repo => repo.GetUserBusyIntervals(userId)).ReturnsAsync(new List<BusyInterval>());
			homeChoreRepositoryMock.Setup(repo => repo.GetUnassignedTasks(homeId)).ReturnsAsync((List<TaskAssignment>)null);

			_calendarController.ControllerContext = new ControllerContext
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
			var result = await _calendarController.AssignTasksToMembers(homeId, googleCheck, busyIntervalCheck, assignedHomeChoresCheck);

			// Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("There are no unassigned tasks available.", badRequestObjectResult.Value);
		}
	}
}
